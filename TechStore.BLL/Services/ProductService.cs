using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Common;
using TechStore.Domain.DTOs.Product;
using TechStore.Domain.Entities;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;
    private static CancellationTokenSource _cacheTokenSource = new();

    public ProductService(IUnitOfWork uow, IMemoryCache cache, IMapper mapper)
    {
        _uow = uow;
        _cache = cache;
        _mapper = mapper;
    }

    #region Queries
    public async Task<Result<PagedResult<ProductSummaryDto>>> GetPagedAsync(ProductFilterDto filter)
    {
        var query = _uow.Products.GetTableNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Where(p => p.IsPublished);

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var q = filter.Query.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(q) || p.SKU.ToLower().Contains(q));
        }

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);

        if (filter.IsFeatured.HasValue)
            query = query.Where(p => p.IsFeatured == filter.IsFeatured.Value);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.BasePrice) >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.BasePrice) <= filter.MaxPrice.Value);

        query = (filter.SortBy?.ToLower()) switch
        {
            "price_asc" => query.OrderBy(p => p.DiscountPrice ?? p.BasePrice),
            "price_desc" => query.OrderByDescending(p => p.DiscountPrice ?? p.BasePrice),
            "name" => query.OrderBy(p => p.Name),
            "best_sellers" => query.OrderByDescending(p => p.SoldCount),
            _ => query.OrderByDescending(p => p.Id)
        };

        var totalCount = await query.CountAsync();
        var products = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        var items = _mapper.Map<List<ProductSummaryDto>>(products);

        return Result<PagedResult<ProductSummaryDto>>.Ok(new PagedResult<ProductSummaryDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    public async Task<Result<ProductDetailDto>> GetByIdAsync(int id)
    {
        var product = await _uow.Products.GetTableNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Specs)
            .Include(p => p.VariantOptions)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return Error.NotFound("Product.NotFound");
        return Result<ProductDetailDto>.Ok(_mapper.Map<ProductDetailDto>(product));
    }

    public async Task<Result<ProductDetailDto>> GetBySlugAsync(string slug)
    {
        var product = await _uow.Products.GetTableNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Specs)
            .Include(p => p.VariantOptions)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (product == null) return Error.NotFound("Product.NotFound");
        return Result<ProductDetailDto>.Ok(_mapper.Map<ProductDetailDto>(product));
    }

    public async Task<Result<IEnumerable<ProductSummaryDto>>> GetFeaturedAsync(int count = 10)
    {
        var products = await _cache.GetOrCreateAsync($"products_featured_{count}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            entry.AddExpirationToken(new CancellationChangeToken(_cacheTokenSource.Token));

            return await _uow.Products.GetTableNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .Where(p => p.IsFeatured && p.IsPublished)
                .OrderByDescending(p => p.Id)
                .Take(count)
                .ToListAsync();
        });

        // No empty check — empty list is a valid response, not an error
        return Result<IEnumerable<ProductSummaryDto>>.Ok(
            _mapper.Map<IEnumerable<ProductSummaryDto>>(products ?? [])
        );
    }

    public async Task<Result<IEnumerable<ProductSummaryDto>>> GetByCategoryAsync(int categoryId, int count = 20)
    {
        // Cache only List<Product> — raw data, no Result wrapper
        var products = await _cache.GetOrCreateAsync($"products_cat_{categoryId}_{count}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            entry.AddExpirationToken(new CancellationChangeToken(_cacheTokenSource.Token));

            return await _uow.Products.GetTableNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .Where(p => p.CategoryId == categoryId && p.IsPublished)
                .OrderByDescending(p => p.Id)
                .Take(count)
                .ToListAsync();
        });

        // Wrap in Result AFTER getting from cache
        return Result<IEnumerable<ProductSummaryDto>>.Ok(
            _mapper.Map<IEnumerable<ProductSummaryDto>>(products)
        );
    }

    public async Task<Result<IEnumerable<ProductSummaryDto>>> GetByBrandAsync(int brandId, int count = 20)
    {
        var products = await _cache.GetOrCreateAsync($"products_brand_{brandId}_{count}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            entry.AddExpirationToken(new CancellationChangeToken(_cacheTokenSource.Token));

            return await _uow.Products.GetTableNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .Where(p => p.BrandId == brandId && p.IsPublished)
                .OrderByDescending(p => p.Id)
                .Take(count)
                .ToListAsync();
        });

        return Result<IEnumerable<ProductSummaryDto>>.Ok(
            _mapper.Map<IEnumerable<ProductSummaryDto>>(products)
        );
    }

    public async Task<Result<IEnumerable<ProductSummaryDto>>> GetRelatedAsync(int productId, int count = 6)
    {
        var productResult = await GetByIdAsync(productId);
        if (!productResult.IsSuccess) return Error.NotFound("Product.NotFound");

        var products = await _uow.Products.GetTableNoTracking()
            .Include(p => p.Brand).Include(p => p.Category).Include(p => p.Images).Include(p => p.Reviews)
            .Where(p => p.CategoryId == productResult.Value.CategoryId && p.Id != productId && p.IsPublished)
            .Take(count)
            .ToListAsync();
        return Result<IEnumerable<ProductSummaryDto>>.Ok(_mapper.Map<IEnumerable<ProductSummaryDto>>(products));
    }

    public async Task<Result<IEnumerable<ProductSummaryDto>>> SearchAsync(string query, int count = 20)
    {
        var filter = new ProductFilterDto { Query = query, Page = 1, PageSize = count };
        var paged = await GetPagedAsync(filter);
        return Result<IEnumerable<ProductSummaryDto>>.Ok(paged.Value.Items);
    }
    #endregion

    #region Commands
    public async Task<Result<ProductDetailDto>> CreateAsync(CreateProductDto dto)
    {
        if (await ExistsAsync(0, dto.Slug)) return Error.Validation("Product.SlugExists", "Slug already exists");

        var product = new Product
        {
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            Slug = dto.Slug,
            SKU = dto.SKU,
            ShortDescription = dto.ShortDescription,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            DiscountPrice = dto.DiscountPrice,
            StockQuantity = dto.StockQuantity,
            HasVariants = dto.HasVariants,
            IsFeatured = dto.IsFeatured,
            IsPublished = dto.IsPublished,
            Label = dto.Label,
            Condition = dto.Condition,
            WarrantyInfo = dto.WarrantyInfo
        };

        await _uow.Products.AddAsync(product);
        await _uow.SaveChangesAsync();

        InvalidateCache();
        return await GetByIdAsync(product.Id);
    }

    public async Task<Result<ProductDetailDto>> UpdateAsync(int id, UpdateProductDto dto)
    {
        if (await ExistsAsync(id, dto.Slug)) return Error.Validation("Product.SlugExists", "Slug already exists");

        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return Error.NotFound("Product.NotFound");

        product.BrandId = dto.BrandId;
        product.CategoryId = dto.CategoryId;
        product.Name = dto.Name;
        product.Slug = dto.Slug;
        product.SKU = dto.SKU;
        product.ShortDescription = dto.ShortDescription;
        product.Description = dto.Description;
        product.BasePrice = dto.BasePrice;
        product.DiscountPrice = dto.DiscountPrice;
        product.StockQuantity = dto.StockQuantity;
        product.HasVariants = dto.HasVariants;
        product.IsFeatured = dto.IsFeatured;
        product.IsPublished = dto.IsPublished;
        product.Label = dto.Label;
        product.Condition = dto.Condition;
        product.WarrantyInfo = dto.WarrantyInfo;

        _uow.Products.Update(product);
        await _uow.SaveChangesAsync();

        InvalidateCache();
        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return Error.NotFound("Product.NotFound");

        _uow.Products.Delete(product);
        await _uow.SaveChangesAsync();

        InvalidateCache();
        return Result.Ok();
    }

    public async Task<Result> SetFeaturedAsync(int id, bool isFeatured)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return Error.NotFound("Product.NotFound");
        product.IsFeatured = isFeatured;
        _uow.Products.Update(product);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result.Ok();
    }

    public async Task<Result> SetPublishedAsync(int id, bool isPublished)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return Error.NotFound("Product.NotFound");
        product.IsPublished = isPublished;
        _uow.Products.Update(product);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result.Ok();
    }

    public async Task<Result> UpdateStockAsync(int id, int quantity)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return Error.NotFound("Product.NotFound");
        product.StockQuantity = quantity;
        _uow.Products.Update(product);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result.Ok();
    }

    public async Task<Result> UpdateLabelAsync(int id, ProductLabel label)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return Error.NotFound("Product.NotFound");
        product.Label = label;
        _uow.Products.Update(product);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result.Ok();
    }
    #endregion

    #region Sub-Entities (Images/Specs/Variants)
    public async Task<Result<ProductImageDto>> AddImageAsync(int productId, AddProductImageDto dto)
    {
        var product = await _uow.Products.GetByIdAsync(productId);
        if (product == null) return Error.NotFound("Product.NotFound");

        var image = new ProductImage
        {
            ProductId = productId,
            ImageUrl = dto.ImageUrl,
            IsMain = dto.IsMain,
            DisplayOrder = dto.DisplayOrder
        };

        if (dto.IsMain)
        {
            var images = await _uow.ProductImages.FindAsync(i => i.ProductId == productId);
            foreach (var img in images) { img.IsMain = false; _uow.ProductImages.Update(img); }
        }

        await _uow.ProductImages.AddAsync(image);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result<ProductImageDto>.Ok(new ProductImageDto { Id = image.Id, ImageUrl = image.ImageUrl, IsMain = image.IsMain, DisplayOrder = image.DisplayOrder });
    }

    public async Task<Result> DeleteImageAsync(int imageId)
    {
        var image = await _uow.ProductImages.GetByIdAsync(imageId);
        if (image == null) return Error.NotFound("ProductImage.NotFound");
        _uow.ProductImages.Delete(image);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result.Ok();
    }

    public async Task<Result> SetMainImageAsync(int productId, int imageId)
    {
        var images = await _uow.ProductImages.FindAsync(i => i.ProductId == productId);
        foreach (var img in images)
        {
            img.IsMain = (img.Id == imageId);
            _uow.ProductImages.Update(img);
        }
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result.Ok();
    }

    public async Task<Result<ProductSpecDto>> AddSpecAsync(int productId, AddProductSpecDto dto)
    {
        var spec = new ProductSpec
        {
            ProductId = productId,
            GroupName = dto.GroupName,
            Name = dto.Name,
            Value = dto.Value,
            DisplayOrder = dto.DisplayOrder
        };
        await _uow.ProductSpecs.AddAsync(spec);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result<ProductSpecDto>.Ok(new ProductSpecDto { Id = spec.Id, Name = spec.Name, Value = spec.Value, DisplayOrder = spec.DisplayOrder });
    }

    public async Task<Result> DeleteSpecAsync(int specId)
    {
        var spec = await _uow.ProductSpecs.GetByIdAsync(specId);
        if (spec == null) return Error.NotFound("ProductSpec.NotFound");
        _uow.ProductSpecs.Delete(spec);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result.Ok();
    }

    public async Task<Result<ProductVariantDto>> AddVariantAsync(int productId, AddProductVariantDto dto)
    {
        var variant = new ProductVariant
        {
            ProductId = productId,
            Name = dto.Name,
            SKU = dto.SKU,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            DiscountPrice = dto.DiscountPrice,
            IsDefault = dto.IsDefault
        };
        await _uow.ProductVariants.AddAsync(variant);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result<ProductVariantDto>.Ok(new ProductVariantDto { Id = variant.Id, SKU = variant.SKU, Price = variant.Price, StockQuantity = variant.StockQuantity });
    }

    public async Task<Result<ProductVariantDto>> UpdateVariantAsync(int variantId, UpdateProductVariantDto dto)
    {
        var variant = await _uow.ProductVariants.GetByIdAsync(variantId);
        if (variant == null) return Error.NotFound("ProductVariant.NotFound");

        variant.Price = dto.Price;
        variant.StockQuantity = dto.StockQuantity;

        _uow.ProductVariants.Update(variant);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result<ProductVariantDto>.Ok(new ProductVariantDto { Id = variant.Id, SKU = variant.SKU, Price = variant.Price, StockQuantity = variant.StockQuantity });
    }

    public async Task<Result> DeleteVariantAsync(int variantId)
    {
        var variant = await _uow.ProductVariants.GetByIdAsync(variantId);
        if (variant == null) return Error.NotFound("ProductVariant.NotFound");
        _uow.ProductVariants.Delete(variant);
        await _uow.SaveChangesAsync();
        InvalidateCache();
        return Result.Ok();
    }
    #endregion

    #region Helpers
    public async Task<Result> AddRelatedProductAsync(int productId, int relatedProductId)
    {
        var rel = new RelatedProduct { ProductId = productId, RelatedProductId = relatedProductId };
        await _uow.RelatedProducts.AddAsync(rel);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> RemoveRelatedProductAsync(int productId, int relatedProductId)
    {
        var rels = await _uow.RelatedProducts.FindAsync(r => r.ProductId == productId && r.RelatedProductId == relatedProductId);
        var rel = rels.FirstOrDefault();
        if (rel != null)
        {
            _uow.RelatedProducts.Delete(rel);
            await _uow.SaveChangesAsync();
        }
        return Result.Ok();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _uow.Products.GetTableNoTracking().AnyAsync(p => p.Id == id);
    }

    private async Task<bool> ExistsAsync(int excludeId, string slug)
    {
        return await _uow.Products.GetTableNoTracking().AnyAsync(p => p.Id != excludeId && p.Slug == slug);
    }

    private void InvalidateCache()
    {
        var oldTokenSource = System.Threading.Interlocked.Exchange(ref _cacheTokenSource, new CancellationTokenSource());
        oldTokenSource?.Dispose();
    }
    #endregion
}
