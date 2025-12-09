using MicroServiceApp.Payment.Api.Repositories;

namespace MicroServiceApp.Payment.Api.Features.Payments.GetAllPaymentsByUserId
{

    public record GetAllPaymentsByUserIdResponse(
        Guid Id,
        string OrderCode,
        string Amount,
        DateTime Created,
        PaymentStatus Status);
}
