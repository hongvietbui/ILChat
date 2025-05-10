using ILChat.Repositories.IRepositories;
using ILChat.Repositories.RepositoryImpls;
using NetCore.AutoRegisterDi;

namespace ILChat.Utilities.Extensions;

public static class AutoDiExtension
{
    public static IServiceCollection AddAutoDiServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.RegisterAssemblyPublicNonGenericClasses()
            .Where(c => c.Name.EndsWith("RepositoryImpl"))
            .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
        
        services.RegisterAssemblyPublicNonGenericClasses()
            .Where(c => c.Name.EndsWith("Service"))
            .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
        return services;
    }
}