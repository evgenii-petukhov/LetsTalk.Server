using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace LetsTalk.Server.Persistence.MongoDBServices;

public static class MongoDBServicesRegistration
{
    public static IServiceCollection AddEntityFrameworkServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));

        return services;
    }
}
