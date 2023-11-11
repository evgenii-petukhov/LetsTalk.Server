using LetsTalk.Server.Persistence.DatabaseAgnosticServices.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public static class EntityFrameworkServiceRegistration
{
    public static IServiceCollection AddEntityFrameworkServices(
        this IServiceCollection services)
    {
        services.AddScoped<IMessageDatabaseAgnosticService, MessageEntityFrameworkService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}
