using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<List<Message>> GetPagedAsync(int chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default);
}
