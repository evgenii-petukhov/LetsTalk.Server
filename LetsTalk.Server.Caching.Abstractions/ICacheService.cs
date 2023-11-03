using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Caching.Abstractions;

public interface ICacheService
{
    Task<List<MessageDto>>? GetOrAddMessagesAsync(int senderId, int recipientId, int pageIndex, Func<Task<List<MessageDto>>> factory);

    Task RemoveMessages(int senderId, int recipientId);

    Task<List<AccountDto>>? GetOrAddAccountsAsync(int accountId, Func<Task<List<AccountDto>>> factory);
}
