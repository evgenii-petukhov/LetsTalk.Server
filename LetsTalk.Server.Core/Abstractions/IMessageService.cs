using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageService
{
    Task<List<MessageDto>> GetPagedAsync(int senderId, int recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken);
}
