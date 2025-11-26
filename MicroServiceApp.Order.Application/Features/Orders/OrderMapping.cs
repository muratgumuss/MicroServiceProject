using AutoMapper;
using MicroServiceApp.Order.Application.Features.Orders.Create;
using MicroServiceApp.Order.Domain.Entities;

namespace MicroServiceApp.Order.Application.Features.Orders
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        }
    }
}
