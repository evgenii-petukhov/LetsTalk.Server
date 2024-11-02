using KafkaFlow;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.API.Core;

public class ClearMessageCacheRequestHandler(IMessageCacheManager messageCacheManager) : IMessageHandler<ClearMessageCacheRequest>
{
    private readonly IMessageCacheManager _messageCacheManager = messageCacheManager;

    public Task Handle(IMessageContext context, ClearMessageCacheRequest message)
    {
        return _messageCacheManager.ClearAsync(message.ChatId!);
    }
}
