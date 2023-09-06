using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Attributes;
using LetsTalk.Server.Core.Services;
using LetsTalk.Server.Persistence.Repository;
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
        services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient<IOpenAuthProvider, FacebookOpenAuthProvider>();
        services.AddTransient<IOpenAuthProvider, VkOpenAuthProvider>();
        services.AddTransient<IRegexService, RegexService>();
        services.AddTransient<IHtmlGenerator, HtmlGenerator>();
        services.AddTransient<IMessageProcessor, MessageProcessor>();
        services.AddTransient<IOpenAuthProviderResolver<string>, OpenAuthProviderResolver<string, OpenAuthProviderIdAttribute>>();
        services.AddPersistenceRepositoryServices(configuration);

        return services;
    }
}
