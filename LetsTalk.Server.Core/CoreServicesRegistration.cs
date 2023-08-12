using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Attributes;
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
        services.AddTransient<IOpenAuthProvider, FacebookOpenAuthProvider>();
        services.AddTransient<IOpenAuthProvider, VkOpenAuthProvider>();
        services.AddTransient<IRegexService, RegexService>();
        services.AddTransient<IHtmlGenerator, HtmlGenerator>();
        services.AddTransient<IMessageProcessor, MessageProcessor>();
        services.AddTransient<IOpenAuthProviderResolver<string>, OpenAuthProviderResolver<string, OpenAuthProviderIdAttribute>>();

        return services;
    }
}
