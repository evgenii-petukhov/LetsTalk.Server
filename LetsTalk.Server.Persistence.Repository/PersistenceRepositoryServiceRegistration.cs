using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Persistence.Repository;

public static class PersistenceRepositoryServiceRegistration
{
    public static IServiceCollection AddPersistenceRepositoryServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistenceServices(configuration);
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<IImageDataLayerService, ImageDataLayerService>();
        services.AddTransient<IMessageRepository, MessageRepository>();
        services.AddTransient<IImageRepository, ImageRepository>();
        services.AddTransient<IFileRepository, FileRepository>();
        services.AddTransient<ILinkPreviewRepository, LinkPreviewRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IEntityFactory, EntityFactory>();
        return services;
    }
}
