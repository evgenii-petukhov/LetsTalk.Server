using LetsTalk.Server.FileStorage.Utility.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Persistence.Repository;

namespace LetsTalk.Server.FileStorage.Utility;

public static class FileStorageUtilityServiceRegistration
{
    public static IServiceCollection AddFileStorageUtilityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IFileNameGenerator, FileNameGenerator>();
        services.AddTransient<IFileStoragePathProvider, FileStoragePathProvider>();
        services.AddTransient<IImageService, ImageService>();
        services.AddPersistenceRepositoryServices(configuration);

        return services;
    }
}
