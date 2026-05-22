using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace LetsTalk.Server.Persistence.Redis
{
    public class RedisConnection(IConfiguration configuration)
    {
        private readonly Lazy<IConnectionMultiplexer> _lazyConnection = new(() =>
        {
            var configurationOptions = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis")!);
            configurationOptions.AbortOnConnectFail = false;

            return ConnectionMultiplexer.Connect(configurationOptions);
        });

        public IConnectionMultiplexer Connection => _lazyConnection.Value;
    }
}
