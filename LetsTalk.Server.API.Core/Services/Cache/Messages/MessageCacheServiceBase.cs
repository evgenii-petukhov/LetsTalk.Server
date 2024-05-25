using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.Messages;

public abstract class MessageCacheServiceBase
{
    protected readonly bool _isActive;

    protected readonly bool _isVolotile;

    protected readonly TimeSpan _cacheLifeTimeInSeconds;

    protected readonly IMessageService _messageService;

    protected MessageCacheServiceBase(
        IMessageService messageService,
        IOptions<CachingSettings> cachingSettings)
    {
        _messageService = messageService;

        _isActive = cachingSettings.Value.MessagesCacheLifeTimeInSeconds != 0;
        _isVolotile = _isActive && cachingSettings.Value.MessagesCacheLifeTimeInSeconds > 0;

        if (_isVolotile)
        {
            _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.MessagesCacheLifeTimeInSeconds);
        }
    }

    protected static string GetMessagePageKey(string chatId)
    {
        return $"messages:{chatId}";
    }

    protected static string GetFirstMessagePageKey(string chatId)
    {
        return $"messages[1st]:{chatId}";
    }
}
