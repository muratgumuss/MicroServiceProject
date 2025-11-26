using MicroServiceApp.Shared;

namespace MicroServiceApp.Order.Application.Features.Orders.GetOrders
{
    public record GetOrdersQuery : IRequestByServiceResult<List<GetOrdersResponse>>;
}
