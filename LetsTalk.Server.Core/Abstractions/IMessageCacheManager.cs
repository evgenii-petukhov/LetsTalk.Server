namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageCacheManager
{
    Task RemoveAsync(string chatId);
}
