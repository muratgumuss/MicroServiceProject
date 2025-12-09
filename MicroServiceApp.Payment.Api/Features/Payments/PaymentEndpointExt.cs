using Asp.Versioning.Builder;
using MicroServiceApp.Payment.Api.Features.Payments.Create;
using MicroServiceApp.Payment.Api.Features.Payments.GetAllPaymentsByUserId;

namespace MicroServiceApp.Payment.Api.Features.Payments
{
    public static class PaymentEndpointExt
    {
        public static void AddPaymentGroupEndpointExt(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            app.MapGroup("api/v{version:apiVersion}/payments").WithTags("payments").WithApiVersionSet(apiVersionSet)
                .CreatePaymentGroupItemEndpoint()
                .GetAllPaymentsByUserIdGroupItemEndpoint();
                //.GetPaymentStatusGroupItemEndpoint();
        }
    }
}
