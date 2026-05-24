using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Local.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.FileStorage.Local.Services;

public static class FileStorageServicesRegistration
{
    public static IServiceCollection AddFileStorageServices(this IServiceCollection services)
    {
        services.AddScoped<IFileNameGenerator, FileNameGenerator>();
        services.AddScoped<IFileStoragePathProvider, FileStoragePathProvider>();
        services.AddScoped<IFileService, FileService>();
        return services;
    }
}
