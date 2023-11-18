using LetsTalk.Server.FileStorage.Utility.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Persistence.EntityFramework.Services;
using LetsTalk.Server.Persistence.MongoDB.Services;

namespace LetsTalk.Server.FileStorage.Utility;

public static class FileStorageUtilityServiceRegistration
{
    public static IServiceCollection AddFileStorageUtilityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IFileNameGenerator, FileNameGenerator>();
        services.AddScoped<IFileStoragePathProvider, FileStoragePathProvider>();
        services.AddScoped<IImageService, ImageService>();

        switch (configuration.GetValue<string>("Database:databaseType"))
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
