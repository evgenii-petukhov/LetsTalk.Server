using LetsTalk.Server.FileStorage.Utility.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Persistence.Repository;
using System.Reflection;
using LetsTalk.Server.Persistence.EntityFrameworkServices;

namespace LetsTalk.Server.FileStorage.Utility;

public static class FileStorageUtilityServiceRegistration
{
    public static IServiceCollection AddFileStorageUtilityServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly assembly)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IFileNameGenerator, FileNameGenerator>();
        services.AddScoped<IFileStoragePathProvider, FileStoragePathProvider>();
        services.AddScoped<IImageService, ImageService>();
        services.AddPersistenceRepositoryServices(configuration, assembly);
        services.AddEntityFrameworkServices(configuration, assembly);

        return services;
    }
}
