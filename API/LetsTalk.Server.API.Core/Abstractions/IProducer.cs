namespace LetsTalk.Server.API.Core.Abstractions;

public interface IProducer<T>
{
    Task PublishAsync(T message, CancellationToken cancellationToken);
}
