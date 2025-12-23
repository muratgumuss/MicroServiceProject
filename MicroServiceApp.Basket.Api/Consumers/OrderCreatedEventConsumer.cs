using MassTransit;
using MicroServiceApp.Basket.Api.Features.Baskets;
using MicroServiceApp.Bus.Events;

namespace MicroServiceApp.Basket.Api.Consumers
{

    public class OrderCreatedEventConsumer(IServiceProvider serviceProvider) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            using var scope = serviceProvider.CreateScope();
            var basketService = scope.ServiceProvider.GetRequiredService<BasketService>();
            await basketService.DeleteBasket(context.Message.UserId);
        }
    }
}
