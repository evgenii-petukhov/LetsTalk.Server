using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Core.Services;
using LetsTalk.Server.Models.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LetsTalk.Server.Core;

public static class CoreServicesRegistration
{
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.Configure<AuthenticationSettings>(configuration.GetSection("AuthenticationSettings"));
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IFacebookService, FacebookService>();
        services.AddTransient<IVkService, VkService>();

        return services;
    }
}
