using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Reflection;
using LetsTalk.Server.Persistence.MongoDB.Repository;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public static class MongoDBServicesRegistration
{
    public static IServiceCollection AddMongoDBServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));

        services.AddScoped<IMessageAgnosticService, MessageMongoDBService>();
        services.AddScoped<IAccountAgnosticService, AccountMongoDBService>();
        services.AddScoped<IImageAgnosticService, ImageMongoDBService>();
        services.AddScoped<IFileAgnosticService, FileMongoDBService>();
        services.AddScoped<ILinkPreviewAgnosticService, LinkPreviewMongoDBService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddMongoDBRepository(configuration);

        return services;
    }
}
