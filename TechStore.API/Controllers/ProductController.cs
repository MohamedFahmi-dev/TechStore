using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Product;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Product.Prefix)]
public class ProductController : AppControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] ProductFilterDto filter)
    {
        var result = await _productService.GetPagedAsync(filter);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Product.GetById)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Product.GetBySlug)]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _productService.GetBySlugAsync(slug);
        return Handle(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return Handle(result, created: true);
    }

    [HttpPut(ApiRoutes.Product.GetById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Product.GetById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);
        return Handle(result);
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int count = 10)
    {
        var result = await _productService.GetFeaturedAsync(count);
        return Handle(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int count = 20)
    {
        var result = await _productService.SearchAsync(q ?? string.Empty, count);
        return Handle(result);
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetByCategory(int categoryId, [FromQuery] int count = 20)
    {
        var result = await _productService.GetByCategoryAsync(categoryId, count);
        return Handle(result);
    }

    [HttpGet("brand/{brandId:int}")]
    public async Task<IActionResult> GetByBrand(int brandId, [FromQuery] int count = 20)
    {
        var result = await _productService.GetByBrandAsync(brandId, count);
        return Handle(result);
    }

    [HttpGet("{id:int}/related")]
    public async Task<IActionResult> GetRelated(int id, [FromQuery] int count = 6)
    {
        var result = await _productService.GetRelatedAsync(id, count);
        return Handle(result);
    }

    [HttpPost("{id:int}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddImage(int id, [FromBody] AddProductImageDto dto)
    {
        var result = await _productService.AddImageAsync(id, dto);
        return Handle(result, created: true);
    }

    [HttpPost("{id:int}/specs")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddSpec(int id, [FromBody] AddProductSpecDto dto)
    {
        var result = await _productService.AddSpecAsync(id, dto);
        return Handle(result, created: true);
    }

    [HttpPost("{id:int}/variants")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddVariant(int id, [FromBody] AddProductVariantDto dto)
    {
        var result = await _productService.AddVariantAsync(id, dto);
        return Handle(result, created: true);
    }
    [HttpPut("{id:int}/images/{imageId:int}/set-main")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetMainImage(int id, int imageId)
    {
        var result = await _productService.SetMainImageAsync(id, imageId);
        return Handle(result);
    }

    [HttpDelete("images/{imageId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveImage(int imageId)
    {
        var result = await _productService.DeleteImageAsync(imageId);
        return Handle(result);
    }

    [HttpDelete("specs/{specId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveSpec(int specId)
    {
        var result = await _productService.DeleteSpecAsync(specId);
        return Handle(result);
    }

    [HttpPut("variants/{variantId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateVariant(int variantId, [FromBody] UpdateProductVariantDto dto)
    {
        var result = await _productService.UpdateVariantAsync(variantId, dto);
        return Handle(result);
    }

    [HttpDelete("variants/{variantId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveVariant(int variantId)
    {
        var result = await _productService.DeleteVariantAsync(variantId);
        return Handle(result);
    }
}
