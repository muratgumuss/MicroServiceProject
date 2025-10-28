using MicroServiceApp.Catalog.Api;
using MicroServiceApp.Catalog.Api.Features.Categories;
using MicroServiceApp.Catalog.Api.Options;
using MicroServiceApp.Catalog.Api.Repositories;
using MicroServiceApp.Shared.Extensions;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptionsExt();
builder.Services.AddDatabaseServiceExt();
builder.Services.AddCommonServiceExt(typeof(CatalogAssembly));

var app = builder.Build();
app.AddCategoryGroupEndpointExt();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();
