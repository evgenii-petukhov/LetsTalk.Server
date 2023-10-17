using LetsTalk.Server.Persistence.Repository.Abstractions;
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
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<IMessageRepository, MessageRepository>();
        services.AddTransient<IImageRepository, ImageRepository>();
        services.AddTransient<IFileRepository, FileRepository>();
        services.AddTransient<ILinkPreviewRepository, LinkPreviewRepository>();
        services.AddTransient<IImageDomainService, ImageDomainService>();
        services.AddTransient<IMessageDomainService, MessageDomainService>();
        services.AddTransient<IAccountDomainService, AccountDomainService>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IEntityFactory, EntityFactory>();
        services.AddMediatR(options => options.RegisterServicesFromAssembly(assembly));
        return services;
    }
}
