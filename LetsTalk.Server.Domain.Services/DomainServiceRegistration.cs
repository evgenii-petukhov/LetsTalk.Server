using LetsTalk.Server.Domain.Abstractions;
using LetsTalk.Server.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Persistence.Repository;

public static class DomainServiceRegistration
{
    public static IServiceCollection AddDomainServices(
        this IServiceCollection services)
    {
        services.AddTransient<IFileFactory, FileFactory>();
        services.AddTransient<IImageFactory, ImageFactory>();
        return services;
    }
}
