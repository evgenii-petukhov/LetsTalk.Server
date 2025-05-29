using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Reflection;
using LetsTalk.Server.Persistence.MongoDB.Repository;
using SimpleMongoMigrations;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public static class MongoDBServicesRegistration
{
    public static async Task<IServiceCollection> AddMongoDBServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));

        services.AddScoped<IMessageAgnosticService, MessageMongoDBService>();
        services.AddScoped<IAccountAgnosticService, AccountMongoDBService>();
        services.AddScoped<IChatAgnosticService, ChatMongoDBService>();
        services.AddScoped<ILinkPreviewAgnosticService, LinkPreviewMongoDBService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddMongoDBRepository(configuration);

        await MigrationEngineBuilder
            .Create()
            .WithConnectionString(configuration.GetConnectionString("MongoDB")!)
            .WithDatabase(configuration.GetValue<string>("MongoDB:DatabaseName")!)
            .WithAssembly(Assembly.GetExecutingAssembly())
            .Build()
            .RunAsync(default);

        return services;
    }
}
