using LetsTalk.Server.Abstractions.Logging;
using LetsTalk.Server.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

        return services;
    }
}
