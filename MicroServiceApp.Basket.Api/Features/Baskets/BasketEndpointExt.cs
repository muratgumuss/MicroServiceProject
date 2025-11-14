using Asp.Versioning.Builder;
using MicroServiceApp.Basket.Api.Features.Baskets.AddBasketItem;

namespace MicroServiceApp.Basket.Api.Features.Baskets
{
    public static class BasketEndpointExt
    {
        public static void AddBasketGroupEndpointExt(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            app.MapGroup("api/v{version:apiVersion}/baskets").WithTags("Baskets")
                .WithApiVersionSet(apiVersionSet)
                .AddBasketItemGroupItemEndpoint();
                //.DeleteBasketItemGroupItemEndpoint()
                //.GetBasketGroupItemEndpoint()
                //.ApplyDiscountCouponGroupItemEndpoint()
                //.RemoveDiscountCouponGroupItemEndpoint()
                //.RequireAuthorization("Password");
        }
    }
}
