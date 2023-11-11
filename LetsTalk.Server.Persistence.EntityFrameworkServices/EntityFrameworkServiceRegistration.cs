using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public static class EntityFrameworkServiceRegistration
{
    public static IServiceCollection AddEntityFrameworkServices(
        this IServiceCollection services)
    {
        services.AddScoped<IMessageAgnosticService, MessageEntityFrameworkService>();
        services.AddScoped<IAccountAgnosticService, AccountEntityFrameworkService>();
        services.AddScoped<IImageAgnosticService, ImageEntityFrameworkService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}
