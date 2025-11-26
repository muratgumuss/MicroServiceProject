using MicroServiceApp.Order.Application.Features.Orders.Create;

namespace MicroServiceApp.Order.Application.Features.Orders.GetOrders
{
    public record GetOrdersResponse(DateTime Created, decimal TotalPrice, List<OrderItemDto> Items);
}
