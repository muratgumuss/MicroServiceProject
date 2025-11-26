namespace MicroServiceApp.Order.Application.Features.Orders.Create
{
    public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice);
}
