using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.ImageProcessor;

public static class ImageProcessorServiceRegistration
{
    public static IServiceCollection AddImageProcessorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}
