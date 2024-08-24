using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Authentication.Services;
using LetsTalk.Server.Authentication.Services.Cache;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Logging;
using StackExchange.Redis;

namespace LetsTalk.Server.Authentication
{
    public static class AuthenticationServiceRegistration
    {
        public static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("all", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddGrpc();
            services.AddGrpcReflection();
            services.AddScoped<IJwtStorageService, JwtStorageService>();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddMemoryCache();
            services.AddLoggingServices();

            services.Configure<CachingSettings>(configuration.GetSection("Caching"));
            services.AddScoped<IJwtStorageService, JwtStorageService>();

            switch (configuration.GetValue<string>("Features:cachingMode"))
            {
                case "redis":
                    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
                    services.AddScoped<IJwtCacheService, RedisCacheTokenService>();
                    break;
                default:
                    services.AddMemoryCache();
                    services.AddScoped<IJwtCacheService, MemoryCacheTokenService>();
                    break;
            }

            return services;
        }
    }
}
