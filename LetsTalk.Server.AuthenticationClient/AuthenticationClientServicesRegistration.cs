using LetsTalk.Server.Abstractions.Authentication;
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
