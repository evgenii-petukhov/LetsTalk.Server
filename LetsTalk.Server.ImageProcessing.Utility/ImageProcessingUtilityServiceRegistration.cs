using LetsTalk.Server.ImageProcessing.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.ImageProcessing.Utility;

public static class ImageProcessingUtilityServiceRegistration
{
    public static IServiceCollection AddImageProcessingUtilityServices(
        this IServiceCollection services)
    {
        services.AddTransient<IImageInfoService, ImageInfoService>();
        services.AddTransient<IImageResizeService, ImageResizeService>();

        return services;
    }
}
