using KafkaFlow;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.API.Core;

public class ClearMessageCacheRequestHandler : IMessageHandler<ClearMessageCacheRequest>
{
    private readonly IMessageCacheManager _messageCacheManager;

    public ClearMessageCacheRequestHandler(IMessageCacheManager messageCacheManager)
    {
        _messageCacheManager = messageCacheManager;
    }

    public Task Handle(IMessageContext context, ClearMessageCacheRequest message)
    {
        return _messageCacheManager.ClearAsync(message.ChatId!);
    }
}
