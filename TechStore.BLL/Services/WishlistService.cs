using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Product;
using TechStore.Domain.DTOs.Wishlist;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Services;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public WishlistService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<WishlistDto>> GetByUserAsync(int userId)
    {
        var items = await _uow.Wishlists
            .GetTableNoTracking()
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .Include(w => w.Product)
                .ThenInclude(p => p.Brand)
            .Include(w => w.Product)
                .ThenInclude(p => p.Reviews)
            .Where(w => w.UserId == userId)
            .ToListAsync();

        var dto = new WishlistDto
        {
            UserId = userId,
            Products = _mapper.Map<IEnumerable<ProductSummaryDto>>(items.Select(w => w.Product))
        };

        return Result<WishlistDto>.Ok(dto);
    }

    public async Task<Result<WishlistDto>> AddProductAsync(int userId, int productId)
    {
        var exists = await _uow.Wishlists.GetTableNoTracking()
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        if (!exists)
        {
            var item = new Wishlist { UserId = userId, ProductId = productId };
            await _uow.Wishlists.AddAsync(item);
            await _uow.SaveChangesAsync();
        }

        return await GetByUserAsync(userId);
    }

    public async Task<Result<WishlistDto>> RemoveProductAsync(int userId, int productId)
    {
        var items = await _uow.Wishlists.FindAsync(
            w => w.UserId == userId && w.ProductId == productId);
        var item = items.FirstOrDefault();

        if (item is null)
            return Error.NotFound("Wishlist.NotFound", "Product not in wishlist.");

        _uow.Wishlists.Delete(item);
        await _uow.SaveChangesAsync();

        return await GetByUserAsync(userId);
    }

    public async Task<Result> ClearAsync(int userId)
    {
        var items = await _uow.Wishlists.FindAsync(w => w.UserId == userId);
        foreach (var item in items)
            _uow.Wishlists.Delete(item);

        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<bool>> IsInWishlistAsync(int userId, int productId)
    {
        var exists = await _uow.Wishlists.GetTableNoTracking()
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        return Result<bool>.Ok(exists);
    }

    public async Task<Result<int>> GetCountAsync(int userId)
    {
        var count = await _uow.Wishlists.GetTableNoTracking()
            .CountAsync(w => w.UserId == userId);
        return Result<int>.Ok(count);
    }
}
