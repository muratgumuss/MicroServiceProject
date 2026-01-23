using MicroServiceApp.Web.Pages.Order.ViewModel;

namespace MicroServiceApp.Web.Pages.Order.Dto
{
    public record GetOrderHistoryResponse(DateTime Created, decimal TotalPrice, List<OrderItemViewModel> Items);
}
