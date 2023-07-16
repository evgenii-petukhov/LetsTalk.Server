using LetsTalk.Server.FileStorage.Utility.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.FileStorage.Utility;

public static class FileStorageUtilityServiceRegistration
{
    public static IServiceCollection AddFileStorageUtilityServices(
        this IServiceCollection services)
    {
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IFileNameGenerator, FileNameGenerator>();
        services.AddTransient<IFileStoragePathProvider, FileStoragePathProvider>();
        services.AddTransient<IImageService, ImageService>();

        return services;
    }
}
