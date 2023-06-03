using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Services;
using LetsTalk.Server.FileStorage.Abstractions;
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
        services.AddTransient<IRegexService, RegexService>();
        services.AddTransient<IBase64ParsingService, Base64ParsingService>();
        services.AddTransient<IHtmlGenerator, HtmlGenerator>();
        services.AddTransient<IMessageProcessor, MessageProcessor>();

        return services;
    }
}
