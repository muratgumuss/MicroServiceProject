using MicroServiceApp.Bus;
using MicroServiceApp.Discount.Api;
using MicroServiceApp.Discount.Api.Features.Discounts;
using MicroServiceApp.Discount.Api.Options;
using MicroServiceApp.Discount.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddVersioningExt();
builder.Services.AddOptionsExt();
builder.Services.AddDatabaseServiceExt();
builder.Services.AddCommonServiceExt(typeof(DiscountAssembly));
builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
builder.Services.AddMasstransitExt(builder.Configuration);
var app = builder.Build();
app.AddDiscountGroupEndpointExt(app.AddVersionSetExt());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(x => { });
app.UseAuthentication();
app.UseAuthorization();
app.Run();
