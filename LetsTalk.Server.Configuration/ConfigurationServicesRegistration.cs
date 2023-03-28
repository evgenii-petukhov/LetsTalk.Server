using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Configuration;

public static class NotificationsServicesRegistration
{
    public static IServiceCollection AddConfigurationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthenticationSettings>(configuration.GetSection("AuthenticationSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));

        return services;
    }
}
