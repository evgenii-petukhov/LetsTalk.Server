using LetsTalk.Server.Authentication.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.AuthenticationClient;

public static class AuthenticationClientServicesRegistration
{
    public static IServiceCollection AddAuthenticationClientServices(
    this IServiceCollection services)
    {
        services.AddTransient<IAuthenticationClient, AuthenticationClient>();

        return services;
    }
}
