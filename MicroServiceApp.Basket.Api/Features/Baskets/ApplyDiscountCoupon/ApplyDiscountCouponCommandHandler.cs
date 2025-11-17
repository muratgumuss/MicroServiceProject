using MediatR;
using MicroServiceApp.Basket.Api.Dto;
using MicroServiceApp.Shared;
using System.Net;
using System.Text.Json;

namespace MicroServiceApp.Basket.Api.Features.Baskets.ApplyDiscountCoupon
{
    public class ApplyDiscountCouponCommandHandler(
       BasketService basketService)
        : IRequestHandler<ApplyDiscountCouponCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(ApplyDiscountCouponCommand request, CancellationToken cancellationToken)
        {
            //var cache = string.Format(BasketConst.BasketCacheKey, identityService.UserId);
            //var basketAsJson = await distributedCache.GetStringAsync(cache, cancellationToken);
            var basketAsJson = await basketService.GetBasketFromCache(cancellationToken);


            if (string.IsNullOrEmpty(basketAsJson))
                return ServiceResult<BasketDto>.Error("Basket not found", HttpStatusCode.NotFound);

            var basket = JsonSerializer.Deserialize<Data.Basket>(basketAsJson)!;

            if (!basket.Items.Any())
                return ServiceResult<BasketDto>.Error("Basket item not found", HttpStatusCode.NotFound);

            basket.ApplyNewDiscount(request.Coupon, request.DiscountRate);

            basketAsJson = JsonSerializer.Serialize(basket);
            //await distributedCache.SetStringAsync(cache, basketAsJson, cancellationToken);
            await basketService.CreateBasketCacheAsync(basket, cancellationToken);

            return ServiceResult.SuccessAsNoContent();
        }
    }
}
