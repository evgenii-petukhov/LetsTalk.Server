using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Persistence.MongoDB.Services;
using LetsTalk.Server.Persistence.EntityFramework.Services;

namespace LetsTalk.Server.Persistence.AgnosticServices;

public static class PersistenceAgnosticServicesRegistration
{
    public static async Task<IServiceCollection> AddPersistenceAgnosticServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        switch (configuration.GetValue<string>("Features:DatabaseMode"))
        {
            case "MongoDB":
                await services.AddMongoDBServices(configuration);
                break;
            default:
                services.AddEntityFrameworkServices(configuration);
                break;
        }

        return services;
    }
}
