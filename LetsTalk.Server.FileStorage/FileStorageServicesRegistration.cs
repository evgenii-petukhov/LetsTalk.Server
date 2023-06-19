using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.FileStorage;

public static class FileStorageServicesRegistration
{
    public static IServiceCollection AddFileStorageServices(
        this IServiceCollection services)
    {
        services.AddTransient<IFileStorageManager, FileStorageManager>();
        services.AddTransient<IImageFileNameGenerator, ImageFileNameGenerator>();
        services.AddTransient<IImageService, ImageService>();
        services.AddTransient<IBase64ParsingService, Base64ParsingService>();

        return services;
    }
}
