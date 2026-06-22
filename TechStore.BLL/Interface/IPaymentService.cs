using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Payment;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Interface;

public interface IPaymentService
{
    Task<Result<PaymentDto>> GetByOrderIdAsync(int orderId);
    Task<Result<PaymentDto>> CreateAsync(int orderId, CreatePaymentDto dto);
    Task<Result<PaymentDto>> UpdateStatusAsync(int orderId, PaymentStatus status, string? transactionId = null);
    Task<Result<PaymentDto>> MarkAsPaidAsync(int orderId, string transactionId);

}

