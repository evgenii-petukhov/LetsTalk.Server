using LetsTalk.Server.Core.Abstractions;
using System.Collections.Concurrent;
using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Services;

public class MessageCacheService: IMessageCacheService
{
    private readonly ConcurrentDictionary<MessageCacheKey, ConcurrentDictionary<int, Task<List<MessageDto>>>> _cache = new();

    public Task<List<MessageDto>>? GetOrAddAsync(int senderId, int recipientId, int pageIndex, Func<Task<List<MessageDto>>> factory)
    {
        var key = new MessageCacheKey
        {
            SenderId = senderId,
            RecipientId = recipientId
        };

        var dict = _cache.GetOrAdd(key, _ => new ConcurrentDictionary<int, Task<List<MessageDto>>>());

        return dict.GetOrAdd(pageIndex, _ => factory());
    }

    public void Remove(int senderId, int recipientId)
    {
        var key = new MessageCacheKey
        {
            SenderId = senderId,
            RecipientId = recipientId
        };

        _cache.TryRemove(key, out _);
    }
}
