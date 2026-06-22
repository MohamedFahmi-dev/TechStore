using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Cart;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<CartDto>> GetByUserAsync(int userId)
    {
        var cart = await _uow.Carts.GetTableNoTracking()
            .Include(c => c.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Images)
            .Include(c => c.Items).ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        if (cart is null)
            return Result<CartDto>.Ok(new CartDto { UserId = userId, Items = new List<CartItemDto>() });

        return Result<CartDto>.Ok(_mapper.Map<CartDto>(cart));
    }

    public async Task<Result<int>> GetItemCountAsync(int userId)
    {
        var cart = await _uow.Carts.GetTableNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        return Result<int>.Ok(cart?.Items?.Sum(i => i.Quantity) ?? 0);
    }

    public async Task<Result<decimal>> GetTotalAsync(int userId)
    {
        var cart = await _uow.Carts.GetTableNoTracking()
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .Include(c => c.Items).ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        return Result<decimal>.Ok(cart?.Items?.Sum(i => i.Quantity * (i.ProductVariant?.Price ?? i.Product?.EffectivePrice ?? 0)) ?? 0);
    }

    public async Task<Result<CartDto>> AddItemAsync(int userId, AddCartItemDto dto)
    {
        var carts = await _uow.Carts.FindAsync(c => c.UserId == userId && c.IsActive);
        var cart = carts.FirstOrDefault();
        
        if (cart == null)
        {
            cart = new Cart { UserId = userId, IsActive = true };
            await _uow.Carts.AddAsync(cart);
            await _uow.SaveChangesAsync();
        }

        int stockAvailable = 0;
        decimal unitPrice = 0;
        if (dto.ProductVariantId.HasValue && dto.ProductVariantId.Value > 0)
        {
            var variant = await _uow.ProductVariants.GetByIdAsync(dto.ProductVariantId.Value);
            if (variant == null) return Error.NotFound("ProductVariant.NotFound", "Variant not found.");
            stockAvailable = variant.StockQuantity;
            unitPrice = variant.Price;
        }
        else
        {
            var product = await _uow.Products.GetByIdAsync(dto.ProductId);
            if (product == null) return Error.NotFound("Product.NotFound", "Product not found.");
            stockAvailable = product.StockQuantity;
            unitPrice = product.EffectivePrice;
        }

        var items = await _uow.CartItems.FindAsync(i => i.CartId == cart.Id);
        var existingItem = items.FirstOrDefault(i => i.ProductId == dto.ProductId && i.ProductVariantId == dto.ProductVariantId);

        var currentQuantity = existingItem?.Quantity ?? 0;
        if (currentQuantity + dto.Quantity > stockAvailable)
            return Error.Validation("Cart.OutOfStock", $"Only {stockAvailable} items available in stock.");

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            existingItem.UnitPrice = unitPrice;
            _uow.CartItems.Update(existingItem);
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                ProductVariantId = dto.ProductVariantId,
                Quantity = dto.Quantity,
                UnitPrice = unitPrice
            };
            await _uow.CartItems.AddAsync(newItem);
        }

        await _uow.SaveChangesAsync();
        return await GetByUserAsync(userId);
    }

    public async Task<Result> ClearAsync(int userId)
    {
        var carts = await _uow.Carts.FindAsync(c => c.UserId == userId && c.IsActive);
        var cart = carts.FirstOrDefault();
        if (cart == null) return Result.Ok();

        var items = await _uow.CartItems.FindAsync(i => i.CartId == cart.Id);
        if (items.Any())
        {
            _uow.CartItems.RemoveRange(items);
            await _uow.SaveChangesAsync();
        }
        return Result.Ok();
    }

    public async Task<Result> DeactivateAsync(int userId)
    {
        var carts = await _uow.Carts.FindAsync(c => c.UserId == userId && c.IsActive);
        var cart = carts.FirstOrDefault();

        if (cart != null)
        {
            cart.IsActive = false;
            _uow.Carts.Update(cart);
            await _uow.SaveChangesAsync();
        }
        return Result.Ok();
    }

    public async Task<Result<CartDto>> RemoveItemAsync(int userId, int cartItemId)
    {
        var carts = await _uow.Carts.FindAsync(c => c.UserId == userId && c.IsActive);
        var cart = carts.FirstOrDefault();
        if (cart == null) return Error.NotFound("Cart.NotFound");

        var items = await _uow.CartItems.FindAsync(i => i.CartId == cart.Id && i.Id == cartItemId);
        var item = items.FirstOrDefault();
        if (item == null) return Error.NotFound("CartItem.NotFound");

        _uow.CartItems.Delete(item);
        await _uow.SaveChangesAsync();
        return await GetByUserAsync(userId);
    }

    public async Task<Result<CartDto>> UpdateItemQuantityAsync(int userId, int cartItemId, int quantity)
    {
        if (quantity <= 0) return await RemoveItemAsync(userId, cartItemId);

        var carts = await _uow.Carts.FindAsync(c => c.UserId == userId && c.IsActive);
        var cart = carts.FirstOrDefault();
        if (cart == null) return Error.NotFound("Cart.NotFound");

        var items = await _uow.CartItems.FindAsync(i => i.CartId == cart.Id && i.Id == cartItemId);
        var item = items.FirstOrDefault();
        if (item == null) return Error.NotFound("CartItem.NotFound");

        int stockAvailable = 0;
        if (item.ProductVariantId.HasValue && item.ProductVariantId.Value > 0)
        {
            var variant = await _uow.ProductVariants.GetByIdAsync(item.ProductVariantId.Value);
            if (variant != null) stockAvailable = variant.StockQuantity;
        }
        else
        {
            var product = await _uow.Products.GetByIdAsync(item.ProductId);
            if (product != null) stockAvailable = product.StockQuantity;
        }

        if (quantity > stockAvailable)
            return Error.Validation("Cart.OutOfStock", $"Only {stockAvailable} items available in stock.");

        item.Quantity = quantity;
        _uow.CartItems.Update(item);
        await _uow.SaveChangesAsync();
        return await GetByUserAsync(userId);
    }
}
