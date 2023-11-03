using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Caching.Abstractions;

public interface ICacheService
{
    Task<List<MessageDto>> GetOrAddMessagesAsync(int senderId, int recipientId, int pageIndex, Func<Task<List<MessageDto>>> factory);

    Task<List<AccountDto>> GetOrAddAccountsAsync(int accountId, Func<Task<List<AccountDto>>> factory);

    Task<ImageCacheEntry> GetOrAddImageAsync(int imageId, Func<Task<ImageCacheEntry>> factory);

    Task RemoveMessagesAsync(int senderId, int recipientId);
}
