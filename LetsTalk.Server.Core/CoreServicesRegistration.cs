using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LetsTalk.Server.Core;

public static class CoreServicesRegistration
{
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IFacebookService, FacebookService>();
        services.AddTransient<IVkService, VkService>();
        services.AddTransient<IMessageProcessor, MessageProcessor>();
        services.AddTransient<IRegexService, RegexService>();

        return services;
    }
}
