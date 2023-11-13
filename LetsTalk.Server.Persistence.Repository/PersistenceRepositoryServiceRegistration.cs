using LetsTalk.Server.Persistence.Repository.Abstractions;
using LetsTalk.Server.Persistence.Repository.DomainServices;
using LetsTalk.Server.Persistence.Repository.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LetsTalk.Server.Persistence.Repository;

public static class PersistenceRepositoryServiceRegistration
{
    public static IServiceCollection AddPersistenceRepositoryServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly assembly)
    {
        services.AddPersistenceServices(configuration);
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<ILinkPreviewRepository, LinkPreviewRepository>();
        services.AddScoped<IMessageDomainService, MessageDomainService>();
        services.AddScoped<IAccountDomainService, AccountDomainService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEntityFactory, EntityFactory>();
        services.AddMediatR(options => options.RegisterServicesFromAssembly(assembly));
        return services;
    }
}
