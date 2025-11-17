using MicroServiceApp.Basket.Api.Dto;
using MicroServiceApp.Shared;

namespace MicroServiceApp.Basket.Api.Features.Baskets.GetBasket
{
    public record GetBasketQuery : IRequestByServiceResult<BasketDto>;
}
