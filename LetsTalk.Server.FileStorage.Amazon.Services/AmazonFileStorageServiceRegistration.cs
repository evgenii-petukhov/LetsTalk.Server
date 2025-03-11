using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.FileStorage.Amazon.Services;

public static class AmazonFileStorageServiceRegistration
{
    public static IServiceCollection AddAmazonFileStorageServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AwsSettings>(configuration.GetSection("Aws"));
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IImageService, ImageService>();
        return services;
    }
}
