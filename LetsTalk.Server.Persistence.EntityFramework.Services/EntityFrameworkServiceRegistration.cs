using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using LetsTalk.Server.Persistence.EntityFramework.Repository;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public static class EntityFrameworkServiceRegistration
{
    public static IServiceCollection AddEntityFrameworkServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IMessageAgnosticService, MessageEntityFrameworkService>();
        services.AddScoped<IAccountAgnosticService, AccountEntityFrameworkService>();
        services.AddScoped<ILinkPreviewAgnosticService, LinkPreviewEntityFrameworkService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddPersistenceRepositoryServices(configuration);

        return services;
    }
}
