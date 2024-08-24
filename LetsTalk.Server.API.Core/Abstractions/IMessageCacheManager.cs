namespace LetsTalk.Server.API.Core.Abstractions;

public interface IMessageCacheManager
{
    Task RemoveAsync(string chatId);
}
