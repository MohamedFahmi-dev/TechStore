using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Common;
using TechStore.Domain.DTOs.Order;
using TechStore.Domain.Entities;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICouponService _couponService;

    public OrderService(IUnitOfWork uow, IMapper mapper, ICouponService couponService)
    {
        _uow = uow;
        _mapper = mapper;
        _couponService = couponService;
    }

    public async Task<Result<OrderDetailDto>> GetByIdAsync(int orderId)
    {
        var order = await LoadOrderQuery()
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
            return Error.NotFound("Order.NotFound", "Order not found.");

        return Result<OrderDetailDto>.Ok(_mapper.Map<OrderDetailDto>(order));
    }

    public async Task<Result<OrderDetailDto>> GetByIdForUserAsync(int orderId, int userId)
    {
        var order = await LoadOrderQuery()
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order is null)
            return Error.NotFound("Order.NotFound", "Order not found.");

        return Result<OrderDetailDto>.Ok(_mapper.Map<OrderDetailDto>(order));
    }

    public async Task<Result<OrderDetailDto>> GetByOrderNumberAsync(string orderNumber)
    {
        var order = await LoadOrderQuery()
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        if (order is null)
            return Error.NotFound("Order.NotFound", $"Order #{orderNumber} not found.");

        return Result<OrderDetailDto>.Ok(_mapper.Map<OrderDetailDto>(order));
    }

    public async Task<Result<PagedResult<OrderSummaryDto>>> GetByUserAsync(int userId, int page = 1, int pageSize = 10)
    {
        var query = _uow.Orders.GetTableNoTracking()
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.Id);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Result<PagedResult<OrderSummaryDto>>.Ok(new PagedResult<OrderSummaryDto>
        {
            Items = _mapper.Map<List<OrderSummaryDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<Result<PagedResult<OrderSummaryDto>>> GetAllAsync(OrderFilterDto filter)
    {
        var query = _uow.Orders.GetTableNoTracking()
            .Include(o => o.Items)
            .AsQueryable();

        if (filter.UserId.HasValue)
            query = query.Where(o => o.UserId == filter.UserId.Value);

        if (filter.Status.HasValue)
            query = query.Where(o => o.OrderStatus == filter.Status.Value);

        if (filter.PaymentStatus.HasValue)
            query = query.Where(o => o.PaymentStatus == filter.PaymentStatus.Value);

        query = query.OrderByDescending(o => o.Id);

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

        return Result<PagedResult<OrderSummaryDto>>.Ok(new PagedResult<OrderSummaryDto>
        {
            Items = _mapper.Map<List<OrderSummaryDto>>(items),
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    public async Task<Result<OrderDetailDto>> PlaceOrderAsync(int userId, PlaceOrderDto dto)
    {
        // Load user cart
        var cart = await _uow.Carts.GetTableNoTracking()
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .Include(c => c.Items).ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        if (cart is null || !cart.Items.Any())
            return Error.Validation("Order.EmptyCart", "Your cart is empty.");

        // Validate address
        var address = await _uow.Addresses.GetTableNoTracking()
            .FirstOrDefaultAsync(a => a.Id == dto.AddressId && a.UserId == userId);

        if (address is null)
            return Error.Validation("Order.InvalidAddress", "Address not found or unauthorized.");

        // Calculate subtotal
        decimal subtotal = cart.Items.Sum(i =>
        {
            var price = i.ProductVariant?.Price ?? i.Product?.EffectivePrice ?? 0;
            return price * i.Quantity;
        });

        // Apply coupon through CouponService (proper validation: dates, limits, minimum order)
        decimal discountAmount = 0;
        int? couponId = null;

        if (!string.IsNullOrWhiteSpace(dto.CouponCode))
        {
            var couponResult = await _couponService.ValidateAsync(dto.CouponCode, subtotal);
            if (!couponResult.IsSuccess)
                return Result<OrderDetailDto>.Fail(couponResult.Errors.ToList());

            discountAmount = couponResult.Value.DiscountAmount;
            couponId = couponResult.Value.CouponId;
        }

        // Calculate shipping server-side (not client-controlled)
        decimal shippingFee = CalculateShippingFee(subtotal);

        var totalAmount = subtotal - discountAmount + shippingFee;

        await _uow.BeginTransactionAsync();

        try
        {
            // Re-verify stock inside transaction with tracked entities (concurrency safety)
            foreach (var item in cart.Items)
            {
                int available;
                if (item.ProductVariantId.HasValue && item.ProductVariantId.Value > 0)
                {
                    var v = await _uow.ProductVariants.GetByIdAsync(item.ProductVariantId.Value);
                    available = v?.StockQuantity ?? 0;
                }
                else
                {
                    var p = await _uow.Products.GetByIdAsync(item.ProductId);
                    available = p?.StockQuantity ?? 0;
                }

                if (item.Quantity > available)
                {
                    await _uow.RollbackTransactionAsync();
                    return Error.Validation("Order.OutOfStock", $"Insufficient stock for {item.Product?.Name}. Available: {available}.");
                }
            }

            var order = new Order
            {
                UserId = userId,
                AddressId = dto.AddressId,
                CouponId = couponId,
                OrderNumber = GenerateOrderNumber(),
                Subtotal = subtotal,
                DiscountAmount = discountAmount,
                ShippingFee = shippingFee,
                TotalAmount = totalAmount,
                Notes = dto.Notes,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            };

            await _uow.Orders.AddAsync(order);
            await _uow.SaveChangesAsync();

            // Create order items + decrement stock + increment SoldCount
            foreach (var cartItem in cart.Items)
            {
                var product = cartItem.Product!;
                var variant = cartItem.ProductVariant;
                var price = variant?.Price ?? product.EffectivePrice;

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    ProductVariantId = cartItem.ProductVariantId,
                    ProductName = product.Name,
                    SKU = variant?.SKU ?? product.SKU,
                    VariantLabel = variant?.Name,
                    UnitPrice = price,
                    Quantity = cartItem.Quantity,
                    TotalPrice = price * cartItem.Quantity
                };
                await _uow.OrderItems.AddAsync(orderItem);

                // Decrement stock
                if (cartItem.ProductVariantId.HasValue && cartItem.ProductVariantId.Value > 0)
                {
                    var v = await _uow.ProductVariants.GetByIdAsync(cartItem.ProductVariantId.Value);
                    if (v != null) { v.StockQuantity -= cartItem.Quantity; _uow.ProductVariants.Update(v); }
                }
                else
                {
                    var p = await _uow.Products.GetByIdAsync(cartItem.ProductId);
                    if (p != null) 
                    { 
                        p.StockQuantity -= cartItem.Quantity; 
                        p.SoldCount += cartItem.Quantity;
                        _uow.Products.Update(p); 
                    }
                }
            }

            // Create payment record
            var payment = new Payment
            {
                OrderId = order.Id,
                Method = dto.PaymentMethod,
                Amount = totalAmount,
                Status = PaymentStatus.Pending
            };
            await _uow.Payments.AddAsync(payment);

            // Increment coupon usage
            if (couponId.HasValue)
            {
                var coupon = await _uow.Coupons.GetByIdAsync(couponId.Value);
                if (coupon is not null) coupon.UsedCount++;
            }

            // Clear cart items
            var cartItemsToDelete = await _uow.CartItems.FindAsync(i => i.CartId == cart.Id);
            foreach (var item in cartItemsToDelete)
                _uow.CartItems.Delete(item);

            await _uow.SaveChangesAsync();
            await _uow.CommitTransactionAsync();

            return await GetByIdAsync(order.Id);
        }
        catch (Exception)
        {
            await _uow.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Result<OrderDetailDto>> UpdateStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order is null)
            return Error.NotFound("Order.NotFound", "Order not found.");

        order.OrderStatus = status;
        await _uow.SaveChangesAsync();
        return await GetByIdAsync(orderId);
    }

    public async Task<Result<OrderDetailDto>> UpdatePaymentStatusAsync(int orderId, PaymentStatus status)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order is null)
            return Error.NotFound("Order.NotFound", "Order not found.");

        order.PaymentStatus = status;
        await _uow.SaveChangesAsync();
        return await GetByIdAsync(orderId);
    }

    public async Task<Result> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _uow.Orders.FindAsync(o => o.Id == orderId && o.UserId == userId);
        var entity = order.FirstOrDefault();

        if (entity is null)
            return Error.NotFound("Order.NotFound", "Order not found or unauthorized.");

        if (entity.OrderStatus != OrderStatus.Pending)
            return Error.Validation("Order.CannotCancel", "Only pending orders can be cancelled.");

        entity.OrderStatus = OrderStatus.Cancelled;

        var items = await _uow.OrderItems.GetTableNoTracking().Where(i => i.OrderId == orderId).ToListAsync();
        foreach (var i in items)
        {
            // Restore stock and decrement SoldCount in single load
            if (i.ProductVariantId.HasValue && i.ProductVariantId.Value > 0)
            {
                var v = await _uow.ProductVariants.GetByIdAsync(i.ProductVariantId.Value);
                if (v != null) { v.StockQuantity += i.Quantity; _uow.ProductVariants.Update(v); }
            }
            else
            {
                // Only load product for non-variant items to decrement SoldCount
                var p = await _uow.Products.GetByIdAsync(i.ProductId);
                if (p != null)
                {
                    p.StockQuantity += i.Quantity;
                    if (p.SoldCount >= i.Quantity)
                    {
                        p.SoldCount -= i.Quantity;
                    }
                    _uow.Products.Update(p);
                }
            }
        }

        if (entity.CouponId.HasValue)
        {
            var coupon = await _uow.Coupons.GetByIdAsync(entity.CouponId.Value);
            if (coupon != null && coupon.UsedCount > 0)
                coupon.UsedCount--;
        }

        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<bool> ExistsAsync(int orderId)
    {
        return await _uow.Orders.GetTableNoTracking().AnyAsync(o => o.Id == orderId);
    }


    private IQueryable<Order> LoadOrderQuery() =>
        _uow.Orders.GetTableNoTracking()
            .Include(o => o.Items)
            .Include(o => o.Address)
            .Include(o => o.Payment)
            .Include(o => o.User)
            .Include(o => o.Coupon);

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

    private static decimal CalculateShippingFee(decimal subtotal)
    {
        const decimal freeShippingThreshold = 2000m; // EGP
        const decimal flatShippingRate = 50m;         // EGP
        return subtotal >= freeShippingThreshold ? 0m : flatShippingRate;
    }
}
