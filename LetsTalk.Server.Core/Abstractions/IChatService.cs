using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IChatService
{
    Task<List<ChatDto>> GetChatsAsync(string accountId, CancellationToken cancellationToken);
}
