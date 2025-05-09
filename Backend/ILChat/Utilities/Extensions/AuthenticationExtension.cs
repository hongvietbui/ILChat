using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ILChat.Utilities.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var realm = configuration["Keycloak:Realm"];
            var baseUrl = configuration["Keycloak:BaseUrl"];
            var authority = baseUrl+ "/realms/" + realm;
            
            options.Authority = authority;
            options.Audience = configuration["Keycloak:ClientId"];
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Keycloak:Authority"],
                ValidateAudience = false,
                
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                
                NameClaimType = "preferred_username",
                // RoleClaimType = "realm_access.roles"
            };
            
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Token failed: {context.Exception}");
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}