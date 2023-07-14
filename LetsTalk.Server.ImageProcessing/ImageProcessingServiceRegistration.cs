using LetsTalk.Server.ImageProcessing.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.ImageProcessing;

public static class ImageProcessingServiceRegistration
{
    public static IServiceCollection AddImageProcessingServices(
        this IServiceCollection services)
    {
        services.AddTransient<IImageInfoService, ImageInfoService>();
        services.AddTransient<IImageResizeService, ImageResizeService>();

        return services;
    }
}
