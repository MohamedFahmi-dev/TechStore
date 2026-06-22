using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Payment;
using TechStore.Domain.Entities;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PaymentDto>> GetByOrderIdAsync(int orderId)
    {
        var payment = await _uow.Payments.GetTableNoTracking()
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        if (payment is null)
            return Error.NotFound("Payment.NotFound", "No payment found for this order.");

        return Result<PaymentDto>.Ok(_mapper.Map<PaymentDto>(payment));
    }

    public async Task<Result<PaymentDto>> CreateAsync(int orderId, CreatePaymentDto dto)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            Method = dto.Method,
            Amount = dto.Amount,
            Status = PaymentStatus.Pending
        };

        await _uow.Payments.AddAsync(payment);
        await _uow.SaveChangesAsync();
        return Result<PaymentDto>.Ok(_mapper.Map<PaymentDto>(payment));
    }

    public async Task<Result<PaymentDto>> MarkAsPaidAsync(int orderId, string transactionId)
    {
        return await UpdateStatusAsync(orderId, PaymentStatus.Paid, transactionId);
    }

    public async Task<Result<PaymentDto>> UpdateStatusAsync(int orderId, PaymentStatus status, string? transactionId = null)
    {
        var payments = await _uow.Payments.FindAsync(p => p.OrderId == orderId);
        var payment = payments.FirstOrDefault();

        if (payment is null)
            return Error.NotFound("Payment.NotFound", "No payment found for this order.");

        payment.Status = status;
        if (transactionId is not null) payment.TransactionId = transactionId;
        if (status == PaymentStatus.Paid) payment.PaidAt = DateTime.UtcNow;

        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order is not null)
            order.PaymentStatus = status;

        await _uow.SaveChangesAsync();
        return Result<PaymentDto>.Ok(_mapper.Map<PaymentDto>(payment));
    }
}
