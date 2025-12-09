using MediatR;
using MicroServiceApp.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MicroServiceApp.Payment.Api.Features.Payments.GetAllPaymentsByUserId
{
    public static class GetAllPaymentsByUserIdEndpoint
    {
        public static RouteGroupBuilder GetAllPaymentsByUserIdGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("",
                    async (IMediator mediator) =>
                        (await mediator.Send(new GetAllPaymentsByUserIdQuery())).ToGenericResult())
                .WithName("get-all-payments-by-userid")
                .MapToApiVersion(1, 0)
                .Produces(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
                //.RequireAuthorization("ClientCredential");

            return group;
        }
    }
}
