using MediatR;
using MicroServiceApp.Basket.Api.Const;
using MicroServiceApp.Basket.Api.Dto;
using MicroServiceApp.Shared;
using MicroServiceApp.Shared.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MicroServiceApp.Basket.Api.Features.Baskets.AddBasketItem
{
    public class AddBasketItemCommandHandler(IDistributedCache distributedCache,
        IIdentityService identityService)
        //BasketService basketService)
        : IRequestHandler<AddBasketItemCommand, ServiceResult>
    {

        public async Task<ServiceResult> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
        {
            Guid userId = identityService.UserId;
            var cacheKey = string.Format(BasketConst.BasketCacheKey, userId);

            var basketAsString = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

            BasketDto? currentBasket;
            var newBasketItem = new BasketItemDto(request.CourseId, request.CourseName, request.ImageUrl,
                request.CoursePrice, null);

            if (string.IsNullOrEmpty(basketAsString))
            {
                currentBasket = new BasketDto(userId, [newBasketItem]);
                await CreateCacheAsync(cacheKey, currentBasket, cancellationToken);

                return ServiceResult.SuccessAsNoContent();
            }
            else
            {
                currentBasket = JsonSerializer.Deserialize<BasketDto>(basketAsString);
                var existingBasketItem = currentBasket!.Items.FirstOrDefault(x => x.Id == request.CourseId);

                if (existingBasketItem is not null)
                {
                    currentBasket.Items.Remove(existingBasketItem);
                    currentBasket.Items.Add(newBasketItem);
                }
                currentBasket.Items.Add(newBasketItem);
                await CreateCacheAsync(cacheKey, currentBasket, cancellationToken);

                return ServiceResult.SuccessAsNoContent();
            }
        }

        private async Task CreateCacheAsync(string cacheKey, BasketDto basket, CancellationToken cancellationToken)
        {
            var basketAsString = JsonSerializer.Serialize(basket);
            await distributedCache.SetStringAsync(cacheKey, basketAsString, cancellationToken);
        }
    }
}
