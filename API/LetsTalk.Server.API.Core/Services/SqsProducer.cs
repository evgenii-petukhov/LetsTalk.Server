using LetsTalk.Server.API.Core.Abstractions;
using MassTransit;

namespace LetsTalk.Server.API.Core.Services;

public class SqsProducer<T>(IBus bus) : IProducer<T>
{
    private readonly IBus _bus = bus;

    public Task PublishAsync(T message, CancellationToken cancellationToken)
    {
        return _bus.Publish(message!, cancellationToken);
    }
}
