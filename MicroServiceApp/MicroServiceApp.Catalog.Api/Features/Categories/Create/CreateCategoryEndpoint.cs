using MediatR;
using MicroServiceApp.Shared.Extensions;

namespace MicroServiceApp.Catalog.Api.Features.Categories.Create
{
    public static class CreateCategoryEndpoint
    {
        public static RouteGroupBuilder CreateCategoryGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapPost("/",
                    async (CreateCategoryCommand command, IMediator mediator) =>
                    {
                        var result = await mediator.Send(command);
                        return result.ToGenericResult();
                    });
                //        (await mediator.Send(command)).ToGenericResult())
                //.WithName("CreateCategory");
                //.MapToApiVersion(1, 0)
                //.AddEndpointFilter<ValidationFilter<CreateCategoryCommand>>();


            return group;
        }
    }
}
