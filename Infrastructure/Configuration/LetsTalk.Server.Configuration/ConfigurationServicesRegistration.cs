using LetsTalk.Server.Configuration.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Configuration.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Configuration;

public static class ConfigurationServicesRegistration
{
    public static IServiceCollection AddConfigurationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IFeaturesSettingsService, FeaturesSettingsService>();
        services.Configure<FeaturesSettings>(configuration.GetSection("Features"));
        return services;
    }
}
