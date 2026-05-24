using LetsTalk.Server.Models.Dtos;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.API.Core.Abstractions;

public interface IMessageService
{
    Task<IReadOnlyList<MessageServiceModel>> GetPagedAsync(string chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken);
}
