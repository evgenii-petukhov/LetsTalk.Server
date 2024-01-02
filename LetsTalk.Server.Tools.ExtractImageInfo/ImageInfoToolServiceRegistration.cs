using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility;
using LetsTalk.Server.ImageProcessing.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Tools.ExtractImageInfo;

public static class ImageInfoToolServiceRegistration
{
    public static IServiceCollection AddImageInfoServices(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddImageProcessingUtilityServices();
        services.AddFileStorageUtilityServices();
        services.AddHostedService<ImageInfoHostedService>();

        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));

        return services;
    }
}