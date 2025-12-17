using MediatR;
using MicroServiceApp.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MicroServiceApp.Payment.Api.Features.Payments.Create
{
    public static class CreatePaymentCommandEndpoint
    {
        public static RouteGroupBuilder CreatePaymentGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapPost("",
                    async ([FromBody] CreatePaymentCommand createPaymentCommand, IMediator mediator) =>
                    (await mediator.Send(createPaymentCommand)).ToGenericResult())
                .WithName("create")
                .MapToApiVersion(1, 0)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .RequireAuthorization("Password");

            return group;
        }
    }
}
