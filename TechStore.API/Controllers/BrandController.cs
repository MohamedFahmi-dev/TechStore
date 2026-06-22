using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Brand;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Brand.Prefix)]
public class BrandController : AppControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _brandService.GetAllAsync();
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Brand.GetActive)]
    public async Task<IActionResult> GetActive()
    {
        var result = await _brandService.GetActiveAsync();
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Brand.GetById)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _brandService.GetByIdAsync(id);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Brand.GetBySlug)]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _brandService.GetBySlugAsync(slug);
        return Handle(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
    {
        var result = await _brandService.CreateAsync(dto);
        return Handle(result, created: true);
    }

    [HttpPut(ApiRoutes.Brand.GetById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandDto dto)
    {
        var result = await _brandService.UpdateAsync(id, dto);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Brand.GetById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _brandService.DeleteAsync(id);
        return Handle(result);
    }
}
