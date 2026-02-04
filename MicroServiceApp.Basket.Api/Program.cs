using MicroServiceApp.Basket.Api;
using MicroServiceApp.Basket.Api.Features.Baskets;
using MicroServiceApp.Bus;
using MicroServiceApp.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCommonServiceExt(typeof(BasketAssembly));
builder.Services.AddMasstransitExt(builder.Configuration);

builder.AddRedisDistributedCache("redis-db-basket");//.net aspire
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("Redis");
//});
builder.Services.AddVersioningExt();
builder.Services.AddScoped<BasketService>();
builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
var app = builder.Build();

app.MapDefaultEndpoints();
app.AddBasketGroupEndpointExt(app.AddVersionSetExt());
app.UseExceptionHandler(x => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.Run();