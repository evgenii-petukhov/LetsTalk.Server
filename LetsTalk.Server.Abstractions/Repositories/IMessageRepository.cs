using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Abstractions.Repositories;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IReadOnlyList<Message>> GetBySenderAndRecipientIdsAsync(int senderId, int recipientId);
}
