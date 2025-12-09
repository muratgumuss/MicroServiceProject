using MicroServiceApp.Shared;

namespace MicroServiceApp.Payment.Api.Features.Payments.GetAllPaymentsByUserId
{
    public record GetAllPaymentsByUserIdQuery : IRequestByServiceResult<List<GetAllPaymentsByUserIdResponse>>;
}
