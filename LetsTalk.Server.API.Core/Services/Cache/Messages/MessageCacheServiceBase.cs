using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.Messages;

public abstract class MessageCacheServiceBase
{
    protected bool IsActive { get; }

    protected bool IsVolatile { get; }

    protected TimeSpan CacheLifeTimeInSeconds { get; }

    protected IMessageService MessageService { get; }

    protected MessageCacheServiceBase(
        IMessageService messageService,
        IOptions<CachingSettings> cachingSettings)
    {
        MessageService = messageService;

        IsActive = cachingSettings.Value.MessagesCacheLifeTimeInSeconds != 0;
        IsVolatile = IsActive && cachingSettings.Value.MessagesCacheLifeTimeInSeconds > 0;

        if (IsVolatile)
        {
            CacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.MessagesCacheLifeTimeInSeconds);
        }
    }

    protected static string GetMessagesKey(string chatId)
    {
        return $"messages:{chatId}";
    }
}
