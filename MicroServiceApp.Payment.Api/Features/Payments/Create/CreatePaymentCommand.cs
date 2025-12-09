using MicroServiceApp.Shared;

namespace MicroServiceApp.Payment.Api.Features.Payments.Create
{
    public record CreatePaymentCommand(
        string OrderCode,
        string CardNumber,
        string CardHolderName,
        string CardExpirationDate,
        string CardSecurityNumber,
        decimal Amount) : IRequestByServiceResult<CreatePaymentResponse>;
}
