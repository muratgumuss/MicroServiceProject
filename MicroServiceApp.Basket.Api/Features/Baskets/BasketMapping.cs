using AutoMapper;
using MicroServiceApp.Basket.Api.Data;
using MicroServiceApp.Basket.Api.Dto;

namespace MicroServiceApp.Basket.Api.Features.Baskets
{
    public class BasketMapping : Profile
    {
        public BasketMapping()
        {
            CreateMap<BasketDto, Data.Basket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();
        }
    }
}
