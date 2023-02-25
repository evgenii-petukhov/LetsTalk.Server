using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Identity.Models;
using LetsTalk.Server.Identity.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Identity;

public static class IdentityServicesRegistration
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IFacebookService, FacebookService>();
        services.AddTransient<IJwtService, JwtService>();

        return services;
    }
}
