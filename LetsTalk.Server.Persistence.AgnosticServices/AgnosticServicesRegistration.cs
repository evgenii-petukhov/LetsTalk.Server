using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Persistence.MongoDB.Services;
using LetsTalk.Server.Persistence.EntityFramework.Services;

namespace LetsTalk.Server.Persistence.AgnosticServices;

public static class AgnosticServicesRegistration
{
    public static IServiceCollection AddAgnosticServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        switch (configuration.GetValue<string>("Features:DatabaseMode"))
        {
            case "MongoDB":
                services.AddMongoDBServices(configuration);
                break;
            default:
                services.AddEntityFrameworkServices(configuration);
                break;
        }

        return services;
    }
}
