using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Category;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Category.Prefix)]
public class CategoryController : AppControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Category.GetTree)]
    public async Task<IActionResult> GetTree()
    {
        var result = await _categoryService.GetTreeAsync();
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Category.GetActive)]
    public async Task<IActionResult> GetActive()
    {
        var result = await _categoryService.GetActiveAsync();
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Category.GetRoots)]
    public async Task<IActionResult> GetRoots()
    {
        var result = await _categoryService.GetRootCategoriesAsync();
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Category.GetChildren)]
    public async Task<IActionResult> GetChildren(int parentId)
    {
        var result = await _categoryService.GetChildrenAsync(parentId);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Category.GetById)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Category.GetBySlug)]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _categoryService.GetBySlugAsync(slug);
        return Handle(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await _categoryService.CreateAsync(dto);
        return Handle(result, created: true);
    }

    [HttpPut(ApiRoutes.Category.GetById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        var result = await _categoryService.UpdateAsync(id, dto);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Category.GetById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);
        return Handle(result);
    }
}
