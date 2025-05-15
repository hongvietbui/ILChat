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
            var clientId = configuration["Keycloak:ClientId"];
            
            options.Authority = authority;
            options.Audience = clientId;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authority,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                NameClaimType = "preferred_username",
            };
            
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context => CheckAzpClaim(context, clientId),
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Token failed: {context.Exception}");
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }

    private static Task CheckAzpClaim(TokenValidatedContext context, string expectedAzp)
    {
        var azp = context.Principal.FindFirst("azp")?.Value;
        if (azp != expectedAzp)
        {
            context.Fail("Invalid azp claim. Expected: " + expectedAzp);
        }

        return Task.CompletedTask;
    }
}