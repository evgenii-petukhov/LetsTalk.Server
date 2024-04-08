using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageService
{
    Task<List<MessageDto>> GetPagedAsync(string senderId, string chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken);
}
