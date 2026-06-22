using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.API.Base;
using TechStore.API.Filters;
using TechStore.BLL.Interface;
using TechStore.Domain.DTOs.Order;
using TechStore.Domain.Routing;

namespace TechStore.API.Controllers;

[Route(ApiRoutes.Order.Prefix)]
[Authorize]
public class OrderController : AppControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [Idempotent]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        var result = await _orderService.PlaceOrderAsync(CurrentUserId, dto);
        return Handle(result, created: true);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        // Ownership check: users can only see their own orders
        var result = await _orderService.GetByIdForUserAsync(id, CurrentUserId);
        return Handle(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _orderService.GetByUserAsync(CurrentUserId, page, pageSize);
        return Handle(result);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilterDto filter)
    {
        var result = await _orderService.GetAllAsync(filter);
        return Handle(result);
    }

    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminGetOrderById(int id)
    {
        // Admin can view any order without ownership check
        var result = await _orderService.GetByIdAsync(id);
        return Handle(result);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var result = await _orderService.CancelOrderAsync(id, CurrentUserId);
        return Handle(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] TechStore.Domain.Enums.OrderStatus status)
    {
        var result = await _orderService.UpdateStatusAsync(id, status);
        return Handle(result);
    }
}
