﻿using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Reflection;
using LetsTalk.Server.Persistence.MongoDB.Repository;
using SimpleMongoMigrations;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public static class MongoDBServicesRegistration
{
    public static IServiceCollection AddMongoDBServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));

        services.AddScoped<IMessageAgnosticService, MessageMongoDBService>();
        services.AddScoped<IAccountAgnosticService, AccountMongoDBService>();
        services.AddScoped<IChatAgnosticService, ChatMongoDBService>();
        services.AddScoped<ILinkPreviewAgnosticService, LinkPreviewMongoDBService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddMongoDBRepository(configuration);

        new MigrationEngine(
            configuration.GetConnectionString("MongoDB")!,
            configuration.GetValue<string>("MongoDB:DatabaseName")!,
            Assembly.GetExecutingAssembly())
            .Run();

        return services;
    }
}
