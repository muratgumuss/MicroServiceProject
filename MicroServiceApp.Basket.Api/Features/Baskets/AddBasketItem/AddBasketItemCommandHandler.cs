using MediatR;
using MicroServiceApp.Shared;
using MicroServiceApp.Shared.Services;
using System.Text.Json;

namespace MicroServiceApp.Basket.Api.Features.Baskets.AddBasketItem
{
    public class AddBasketItemCommandHandler(
        IIdentityService identityService,
        BasketService basketService)
        : IRequestHandler<AddBasketItemCommand, ServiceResult>
    {

        public async Task<ServiceResult> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
        {
            var basketAsJson = await basketService.GetBasketFromCache(cancellationToken);

            Data.Basket? currentBasket;
            var newBasketItem = new Data.BasketItem(request.CourseId, request.CourseName, request.ImageUrl,
                request.CoursePrice, null);

            if (string.IsNullOrEmpty(basketAsJson))
            {
                currentBasket = new Data.Basket(identityService.UserId, [newBasketItem]);
                await basketService.CreateBasketCacheAsync(currentBasket, cancellationToken);

                return ServiceResult.SuccessAsNoContent();
            }
            else
            {
                currentBasket = JsonSerializer.Deserialize<Data.Basket>(basketAsJson);
                var existingBasketItem = currentBasket!.Items.FirstOrDefault(x => x.Id == request.CourseId);

                if (existingBasketItem is not null)
                {
                    currentBasket.Items.Remove(existingBasketItem);
                    currentBasket.Items.Add(newBasketItem);
                }
                currentBasket.Items.Add(newBasketItem);
                await basketService.CreateBasketCacheAsync(currentBasket, cancellationToken);

                return ServiceResult.SuccessAsNoContent();
            }
        }

        //private async Task CreateCacheAsync(string cacheKey, Data.Basket basket, CancellationToken cancellationToken)
        //{
        //    var basketAsString = JsonSerializer.Serialize(basket);
        //    await distributedCache.SetStringAsync(cacheKey, basketAsString, cancellationToken);
        //}
    }
}
