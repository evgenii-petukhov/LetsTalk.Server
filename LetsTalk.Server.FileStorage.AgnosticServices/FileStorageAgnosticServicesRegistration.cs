using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.FileStorage.Local.Services;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.FileStorage.AgnosticServices.Services;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Amazon.Services;
using LetsTalk.Server.FileStorage.Local.Services.Abstractions;

namespace LetsTalk.Server.FileStorage.AgnosticServices;

public static class FileStorageAgnosticServicesRegistration
{
    public static IServiceCollection AddFileStorageAgnosticServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IFileNameGenerator, FileNameGenerator>();
        services.AddScoped<IFileStoragePathProvider, FileStoragePathProvider>();
        switch (configuration.GetValue<string>("Features:FileStorage"))
        {
            case "aws":
                services.Configure<AwsSettings>(configuration.GetSection("Aws"));
                services.AddScoped<IAgnosticFileService, AmazonFallbackFileService>();
                services.AddScoped<IAmazonFileService, AmazonFileService>();
                services.AddScoped<IFileService, FileService>();
                break;
            default:
                services.AddScoped<IAgnosticFileService, FileService>();
                break;
        }

        return services;
    }
}
