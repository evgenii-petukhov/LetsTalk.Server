using LetsTalk.Server.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Logging;

public static class LoggingServicesRegistration
{
    public static IServiceCollection AddLoggingServices(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

        return services;
    }
}
