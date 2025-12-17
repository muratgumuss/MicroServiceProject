using MicroServiceApp.Order.Api.Endpoints.Orders;
using MicroServiceApp.Order.Application;
using MicroServiceApp.Order.Application.Contracts.Repositories;
using MicroServiceApp.Order.Application.Contracts.UnitOfWork;
using MicroServiceApp.Order.Persistence;
using MicroServiceApp.Order.Persistence.Repositories;
using MicroServiceApp.Order.Persistence.UnitOfWork;
using MicroServiceApp.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCommonServiceExt(typeof(OrderApplicationAssembly));
//builder.Services.AddCommonMasstransitExt(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddVersioningExt();
builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
//builder.Services.AddRefitConfigurationExt(builder.Configuration);

//builder.Services.AddHostedService<CheckPaymentStatusOrderBackgroundService>();
// Add services to the container.

var app = builder.Build();
app.AddOrderGroupEndpointExt(app.AddVersionSetExt());
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) app.MapOpenApi();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.Run();