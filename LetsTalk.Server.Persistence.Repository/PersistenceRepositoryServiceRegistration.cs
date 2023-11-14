using LetsTalk.Server.Persistence.Repository.Abstractions;
using LetsTalk.Server.Persistence.Repository.Repositories;
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
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<ILinkPreviewRepository, LinkPreviewRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEntityFactory, EntityFactory>();
        return services;
    }
}
