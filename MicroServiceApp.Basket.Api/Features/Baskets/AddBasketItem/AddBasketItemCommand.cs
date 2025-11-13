using MicroServiceApp.Shared;

namespace MicroServiceApp.Basket.Api.Features.Baskets.AddBasketItem
{

    public record AddBasketItemCommand(Guid CourseId, string CourseName, decimal CoursePrice, string? ImageUrl)
        : IRequestByServiceResult;
}
