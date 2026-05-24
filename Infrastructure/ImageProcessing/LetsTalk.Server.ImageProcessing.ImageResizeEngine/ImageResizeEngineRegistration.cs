using LetsTalk.Server.ImageProcessing.ImageResizeEngine.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.ImageProcessing.ImageResizeEngine;

public static class ImageResizeEngineRegistration
{
    public static IServiceCollection AddImageResizeEngineServices(
        this IServiceCollection services)
    {
        services.AddScoped<IImageInfoService, ImageInfoService>();
        services.AddScoped<IImageResizeService, ImageResizeService>();

        return services;
    }
}
