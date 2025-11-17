using AutoMapper;
using MediatR;
using MicroServiceApp.Basket.Api.Const;
using MicroServiceApp.Basket.Api.Dto;
using MicroServiceApp.Shared;
using MicroServiceApp.Shared.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MicroServiceApp.Basket.Api.Features.Baskets.GetBasket
{
    public class GetBasketQueryHandler(
        IMapper mapper,
        BasketService basketService)
        : IRequestHandler<GetBasketQuery, ServiceResult<BasketDto>>
    {
        public async Task<ServiceResult<BasketDto>> Handle(GetBasketQuery request,
            CancellationToken cancellationToken)
        {
            var basketAsJson = await basketService.GetBasketFromCache(cancellationToken);

            if (string.IsNullOrEmpty(basketAsJson))
            {
                return ServiceResult<BasketDto>.Error("Basket not found", HttpStatusCode.NotFound);
            }
            var basket = JsonSerializer.Deserialize<Data.Basket>(basketAsJson);
            var basketDto = mapper.Map<BasketDto>(basket);

            return ServiceResult<BasketDto>.SuccessAsOk(basketDto);
        }
    }
}
