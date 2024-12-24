using LetsTalk.Server.API.Core.Abstractions;
using MassTransit;

namespace LetsTalk.Server.API.Core.Services;

public class KafkaProducer<T>(ITopicProducer<string, T> topicProducer) : IProducer<T>
    where T : class
{
    private readonly ITopicProducer<string, T> _topicProducer = topicProducer;

    public Task PublishAsync(T message, CancellationToken cancellationToken)
    {
        return _topicProducer.Produce(Guid.NewGuid().ToString(), message!, cancellationToken);
    }
}
