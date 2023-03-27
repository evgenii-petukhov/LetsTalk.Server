using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.AuthenticationClient;

public static class AuthenticationClientServicesRegistration
{
    public static IServiceCollection AddAuthenticationClientServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IAuthenticationClient, AuthenticationClient>();
        services.Configure<AuthenticationSettings>(configuration.GetSection("AuthenticationSettings"));

        return services;
    }
}
