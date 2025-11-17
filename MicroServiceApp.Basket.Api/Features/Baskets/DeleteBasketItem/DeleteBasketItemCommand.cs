using MicroServiceApp.Shared;

namespace MicroServiceApp.Basket.Api.Features.Baskets.DeleteBasketItem
{
    public record DeleteBasketItemCommand(Guid Id) : IRequestByServiceResult;
}
