using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.GrpcInterceptors;
using LetsTalk.Server.FileStorageService.Services;
using LetsTalk.Server.Logging;

namespace LetsTalk.Server.FileStorageService;

public static class FileStorageServiceRegistration
{
    public static IServiceCollection AddFileStorageServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
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
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IFileNameGenerator, FileNameGenerator>();
        services.AddTransient<IImageInfoService, ImageInfoService>();
        services.AddTransient<IImageService, ImageService>();
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.AddAuthenticationClientServices(configuration);
        services.AddLoggingServices();
        services.AddMemoryCache();
        return services;
    }
}
