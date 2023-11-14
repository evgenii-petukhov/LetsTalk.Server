using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Persistence.Repository;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public static class EntityFrameworkServiceRegistration
{
    public static IServiceCollection AddEntityFrameworkServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IMessageAgnosticService, MessageEntityFrameworkService>();
        services.AddScoped<IAccountAgnosticService, AccountEntityFrameworkService>();
        services.AddScoped<IImageAgnosticService, ImageEntityFrameworkService>();
        services.AddScoped<IFileAgnosticService, FileEntityFrameworkService>();
        services.AddScoped<ILinkPreviewAgnosticService, LinkPreviewEntityFrameworkService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddPersistenceRepositoryServices(configuration);

        return services;
    }
}
