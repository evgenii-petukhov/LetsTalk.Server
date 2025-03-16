using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.FileStorage.Amazon.Services;

public static class AmazonFileServicesRegistration
{
    public static IServiceCollection AddAmazonFileServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IFileService, AmazonFileService>();
        services.Configure<AwsSettings>(configuration.GetSection("Aws"));
        return services;
    }
}
