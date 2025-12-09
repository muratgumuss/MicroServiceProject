namespace MicroServiceApp.Payment.Api.Features.Payments.Create
{
    public record CreatePaymentResponse(Guid? PaymentId, bool Status, string? ErrorMessage);
}
