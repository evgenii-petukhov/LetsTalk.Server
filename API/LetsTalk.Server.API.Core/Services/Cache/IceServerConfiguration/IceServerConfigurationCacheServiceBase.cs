using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.IceServerConfiguration;

public abstract class IceServerConfigurationCacheServiceBase(
    IIceServerConfigurationService iceServerConfigurationService,
    IOptions<CloudflareSettings> options)
{
    protected const string Key = "iceServers";

    protected IIceServerConfigurationService IceServerConfigurationService { get; } = iceServerConfigurationService;

    protected TimeSpan CacheLifeTimeInSeconds { get; } = TimeSpan.FromSeconds(options.Value.TokenTtl);
}
