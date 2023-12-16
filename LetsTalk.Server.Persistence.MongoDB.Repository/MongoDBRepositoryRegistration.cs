using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public static class MongoDBRepositoryRegistration
{
    public static IServiceCollection AddMongoDBRepository(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<ILinkPreviewRepository, LinkPreviewRepository>();
        services.AddScoped<IUploadRepository, UploadRepository>();
        services.Configure<DatabaseSettings>(configuration.GetSection("Database"));

        return services;
    }
}
