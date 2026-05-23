using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.AgnosticServices;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Tools.ExtractImageInfo;

public static class ImageInfoToolServiceRegistration
{
    public static IServiceCollection AddImageInfoServices(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddImageResizeEngineServices();
        services.AddFileStorageAgnosticServices(configuration);
        services.AddHostedService<ImageInfoHostedService>();

        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));

        return services;
    }
}