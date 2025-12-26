using MicroServiceApp.Shared;

namespace MicroServiceApp.Payment.Api.Features.Payments.GetStatus
{
    public record GetPaymentStatusRequest(string orderCode) : IRequestByServiceResult<GetPaymentStatusResponse>;
}
