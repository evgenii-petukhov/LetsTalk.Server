using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.FileStorage.Local.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.FileStorage.Local.Services;

public static class FileStorageServiceRegistration
{
    public static IServiceCollection AddFileStorageServices(
        this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IFileNameGenerator, FileNameGenerator>();
        services.AddScoped<IFileStoragePathProvider, FileStoragePathProvider>();

        return services;
    }
}
