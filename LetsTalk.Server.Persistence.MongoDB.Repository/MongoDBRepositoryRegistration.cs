using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public static class MongoDBRepositoryRegistration
{
    public static IServiceCollection AddMongoDBRepository(
        this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
}
