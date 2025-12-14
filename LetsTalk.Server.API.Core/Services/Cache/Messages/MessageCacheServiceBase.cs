using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.Messages;

public abstract class MessageCacheServiceBase
{
    protected bool IsActive { get; }

    protected bool IsVolotile { get; }

    protected TimeSpan CacheLifeTimeInSeconds { get; }

    protected IMessageService MessageService { get; }

    protected MessageCacheServiceBase(
        IMessageService messageService,
        IOptions<CachingSettings> cachingSettings)
    {
        MessageService = messageService;

        IsActive = cachingSettings.Value.MessagesCacheLifeTimeInSeconds != 0;
        IsVolotile = IsActive && cachingSettings.Value.MessagesCacheLifeTimeInSeconds > 0;

        if (IsVolotile)
        {
            CacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.MessagesCacheLifeTimeInSeconds);
        }
    }

    protected static string GetFirstMessagePageKey(string chatId)
    {
        return $"messages[1st]:{chatId}";
    }
}
