using System.Net;
using System.Text.Json;
using MediatR;
using MicroServiceApp.Shared;

namespace MicroServiceApp.Basket.Api.Features.Baskets.DeleteBasketItem
{
    public class DeleteBasketItemCommandHandler(
        BasketService basketService)
        : IRequestHandler<DeleteBasketItemCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteBasketItemCommand request, CancellationToken cancellationToken)
        {
            //var basketAsString = await distributedCache.GetStringAsync(cacheKey, cancellationToken);
            var basketAsJson = await basketService.GetBasketFromCache(cancellationToken);
            if (string.IsNullOrEmpty(basketAsJson))
            {
                return ServiceResult.Error("Basket Not Found",HttpStatusCode.NotFound);
            }
            var currentBasket = JsonSerializer.Deserialize<Data.Basket>(basketAsJson);
            var basketItemToDelete = currentBasket!.Items.FirstOrDefault(x => x.Id == request.Id);

            if (basketItemToDelete == null)
            {
                return ServiceResult.Error("Basket Item Not Found",HttpStatusCode.NotFound);
            }
            currentBasket.Items.Remove(basketItemToDelete);
            basketAsJson = JsonSerializer.Serialize(currentBasket);
            //await distributedCache.SetStringAsync(cacheKey, basketAsString, cancellationToken);
            await basketService.CreateBasketCacheAsync(currentBasket, cancellationToken);

            return ServiceResult.SuccessAsNoContent();
        }
    }
}
