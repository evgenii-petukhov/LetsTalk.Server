using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repositories;
using LetsTalk.Server.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<LetsTalkDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DbConnectionString");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<IAccountDataLayerService, AccountDataLayerService>();
        services.AddTransient<IMessageRepository, MessageRepository>();
        services.AddTransient<IImageRepository, ImageRepository>();
        services.AddTransient<ILinkPreviewRepository, LinkPreviewRepository>();
        return services;
    }
}
