using MicroServiceApp.Order.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.


app.Run();