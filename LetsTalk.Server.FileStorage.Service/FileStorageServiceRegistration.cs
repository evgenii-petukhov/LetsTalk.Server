using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.GrpcInterceptors;
using LetsTalk.Server.FileStorage.Service.Services;
using LetsTalk.Server.Logging;
using LetsTalk.Server.FileStorage.Utility;
using LetsTalk.Server.ImageProcessing.Utility;

namespace LetsTalk.Server.FileStorage.Service;

public static class FileStorageServiceRegistration
{
    public static IServiceCollection AddFileStorageServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);

        services.AddCors(options =>
        {
            options.AddPolicy("all", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        services.AddGrpc(options => options.Interceptors.Add<JwtInterceptor>());
        services.AddGrpcReflection();
        services.AddTransient<IImageValidationService, ImageValidationService>();
        services.AddTransient<IIOService, IOService>();
        services.AddAuthenticationClientServices(configuration);
        services.AddLoggingServices();
        services.AddFileStorageUtilityServices();
        services.AddImageProcessingUtilityServices();
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));

        return services;
    }
}
