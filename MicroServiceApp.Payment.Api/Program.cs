using MicroServiceApp.Bus;
using MicroServiceApp.Payment.Api;
using MicroServiceApp.Payment.Api.Features.Payments;
using MicroServiceApp.Payment.Api.Repositories;
using MicroServiceApp.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddVersioningExt();
builder.Services.AddCommonServiceExt(typeof(PaymentAssembly));
builder.Services.AddCommonMasstransitExt(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("payment-in-memory-db"); });

// Add services to the container.

builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);


var app = builder.Build();
app.AddPaymentGroupEndpointExt(app.AddVersionSetExt());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
   // app.MapOpenApi();
}
app.UseExceptionHandler(x => { });
app.UseAuthentication();
app.UseAuthorization();
app.Run();
