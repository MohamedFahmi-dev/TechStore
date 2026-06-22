using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Common;
using TechStore.Domain.DTOs.Order;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Interface;

public interface IOrderService
{
    Task<Result<PagedResult<OrderSummaryDto>>> GetByUserAsync(int userId, int page = 1, int pageSize = 10);
    Task<Result<OrderDetailDto>> GetByIdAsync(int orderId);
    Task<Result<OrderDetailDto>> GetByIdForUserAsync(int orderId, int userId);
    Task<Result<OrderDetailDto>> GetByOrderNumberAsync(string orderNumber);
    Task<Result<OrderDetailDto>> PlaceOrderAsync(int userId, PlaceOrderDto dto);
    Task<Result> CancelOrderAsync(int orderId, int userId);
    Task<Result<PagedResult<OrderSummaryDto>>> GetAllAsync(OrderFilterDto filter);
    Task<Result<OrderDetailDto>> UpdateStatusAsync(int orderId, OrderStatus status);
    Task<Result<OrderDetailDto>> UpdatePaymentStatusAsync(int orderId, PaymentStatus status);

    Task<bool> ExistsAsync(int orderId);
}

