using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Homepage;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Homepage.Prefix)]
public class HomepageController : AppControllerBase
{
    private readonly IHomepageService _homepageService;

    public HomepageController(IHomepageService homepageService)
    {
        _homepageService = homepageService;
    }

    [HttpGet(ApiRoutes.Homepage.Setup)]
    public async Task<IActionResult> GetFullSetup()
    {
        var result = await _homepageService.GetActiveSectionsAsync();
        return Handle(result);
    }

    [HttpGet(ApiRoutes.Homepage.Sections)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllSections()
    {
        var result = await _homepageService.GetAllSectionsAsync();
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Homepage.Sections)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSection([FromBody] CreateHomepageSectionDto dto)
    {
        var result = await _homepageService.CreateSectionAsync(dto);
        return Handle(result, created: true);
    }

    [HttpPut(ApiRoutes.Homepage.SectionById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSection(int id, [FromBody] UpdateHomepageSectionDto dto)
    {
        var result = await _homepageService.UpdateSectionAsync(id, dto);
        return Handle(result);
    }

    [HttpDelete(ApiRoutes.Homepage.SectionById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSection(int id)
    {
        var result = await _homepageService.DeleteSectionAsync(id);
        return Handle(result);
    }

    [HttpPost(ApiRoutes.Homepage.SectionItems)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddItemToSection(int sectionId, [FromBody] TechStore.Domain.DTOs.Homepage.AddSectionItemDto dto)
    {
        var result = await _homepageService.AddItemAsync(sectionId, dto);
        return Handle(result, created: true);
    }

    [HttpDelete(ApiRoutes.Homepage.SectionItemById)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveItemFromSection(int sectionId, int itemId)
    {
        var result = await _homepageService.RemoveItemAsync(itemId);
        return Handle(result);
    }
}
