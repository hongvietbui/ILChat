using ILChat.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ILChat.Utilities.Extensions;

public static class DatabaseConfigurationExtension
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = configuration.GetSection("ConnectionStrings:MongoDb").Value;
            return new MongoClient(settings);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var databaseName = configuration.GetSection("MongoDbSettings:DatabaseName").Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
        
        services.AddDbContext<ChatDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"), sqlOptions =>
            {
                sqlOptions.CommandTimeout(60);
                sqlOptions.EnableRetryOnFailure();
            });
        });
        return services;
    }
}