using LetsTalk.Server.Caching.Abstractions.Models;

namespace LetsTalk.Server.Caching.Abstractions;

public interface ICacheService
{
    Task<List<MessageCacheEntry>> GetOrAddMessagesAsync(int senderId, int recipientId, int pageIndex, Func<Task<List<MessageCacheEntry>>> factory);

    Task<List<AccountCacheEntry>> GetOrAddAccountsAsync(int accountId, Func<Task<List<AccountCacheEntry>>> factory);

    Task<ImageCacheEntry> GetOrAddImageAsync(int imageId, Func<Task<ImageCacheEntry>> factory);

    Task RemoveMessagesAsync(int senderId, int recipientId);
}
