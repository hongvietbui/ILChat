using ILChat.Entities;
using ILChat.Mappings;
using ILChat.Utilities;
using ILChat.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoDiServices();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(MapProfile).Assembly);

// Add services to the container.

builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.AddMapGrpcServiceExtension();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();