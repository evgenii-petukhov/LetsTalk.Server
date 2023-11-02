using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageCacheService
{
    Task<List<MessageDto>>? GetOrAddAsync(int senderId, int recipientId, int pageIndex, Func<Task<List<MessageDto>>> factory);

    void Remove(int senderId, int recipientId);
}
