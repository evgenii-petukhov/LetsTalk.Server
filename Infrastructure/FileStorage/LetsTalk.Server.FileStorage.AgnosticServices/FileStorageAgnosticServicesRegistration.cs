using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.FileStorage.Local.Services;
using LetsTalk.Server.FileStorage.Amazon.Services;
using LetsTalk.Server.FileStorage.Abstractions;
using Microsoft.Extensions.Configuration;
using LetsTalk.Server.Configuration;

namespace LetsTalk.Server.FileStorage.AgnosticServices;

public static class FileStorageAgnosticServicesRegistration
{
    public static IServiceCollection AddFileStorageAgnosticServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddFileStorageServices();
        services.AddAmazonFileServices(configuration);
        services.AddConfigurationServices(configuration);
        services.AddScoped<IFileServiceResolver, FileServiceResolver>();

        return services;
    }
}
