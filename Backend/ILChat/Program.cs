using ILChat.Entities;
using ILChat.Mappings;
using ILChat.Repositories.IRepositories;
using ILChat.Repositories.RepositoryImpls;
using ILChat.Utilities;
using ILChat.Services;
using ILChat.Utilities.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using NetCore.AutoRegisterDi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoDiServices();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration.GetSection("ConnectionStrings:MongoDb").Value;
    return new MongoClient(settings);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ChatDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), sqlOptions =>
    {
        sqlOptions.CommandTimeout(60);
        sqlOptions.EnableRetryOnFailure();
    });
});

builder.Services.AddAutoMapper(typeof(MapProfile).Assembly);

// Add services to the container.

builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UserService>();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();