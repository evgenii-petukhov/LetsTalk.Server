using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.FileStorage.Local.Services;
using LetsTalk.Server.FileStorage.Amazon.Services;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;

namespace LetsTalk.Server.FileStorage.AgnosticServices;

public static class FileStorageAgnosticServicesRegistration
{
    public static IServiceCollection AddFileStorageAgnosticServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IImageService, ImageService>();
        switch (configuration.GetValue<string>("Features:FileStorage"))
        {
            case "aws":
                services.AddAmazonFileStorageServices(configuration);
                break;
            default:
                services.AddFileStorageServices();
                break;
        }

        return services;
    }
}
