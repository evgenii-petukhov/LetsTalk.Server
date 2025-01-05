using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.SignPackage.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.SignPackage;

public static class SignPackageServiceRegistration
{
    public static IServiceCollection AddSignPackageServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SignPackageSettings>(configuration.GetSection("SignPackage"));
        services.AddScoped<ISignPackageService, SignPackageService>();

        return services;
    }
}
