using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.FileStorage.Local.Services;

namespace LetsTalk.Server.FileStorage.AgnosticServices;

public static class FileStorageAgnosticServicesRegistration
{
    public static IServiceCollection AddFileStorageAgnosticServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        switch (configuration.GetValue<string>("Features:FileStorage"))
        {
            case "aws":
                break;
            default:
                services.AddLocalFileStorageServices();
                break;
        }

        return services;
    }
}
