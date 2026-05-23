using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Authentication.Services.Cache.LoginCodes;
using LetsTalk.Server.Authentication.Services.Cache.Token;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Logging;
using LetsTalk.Server.Persistence.Redis;

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
            services.AddScoped<ILoginCodeGenerator, LoginCodeGenerator>();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddMemoryCache();
            services.AddLoggingServices();
            services.Configure<CachingSettings>(configuration.GetSection("Caching"));
            switch (configuration.GetValue<string>("Features:CachingMode"))
            {
                case "redis":
                    services.AddRedisCache();
                    services.AddScoped<IJwtCacheService, RedisCacheTokenService>();
                    services.AddScoped<ILoginCodeCacheService, LoginCodeRedisCacheService>();
                    break;
                default:
                    services.AddMemoryCache();
                    services.AddScoped<IJwtCacheService, MemoryCacheTokenService>();
                    services.AddScoped<ILoginCodeCacheService, LoginCodeMemoryCacheService>();
                    break;
            }

            return services;
        }
    }
}
