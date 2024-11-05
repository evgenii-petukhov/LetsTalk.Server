using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.Persistence.Redis
{
    public static class RedisRegistration
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services)
        {
            services.AddSingleton<RedisConnection>();

            return services;
        }
    }
}
