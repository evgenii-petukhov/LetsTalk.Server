using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IChatMessageStatusRepository
{
    Task CreateAsync(ChatMessageStatus entity, CancellationToken cancellationToken = default);
}
