namespace LetsTalk.Server.API.Core.Abstractions;

public interface IMessageCacheManager
{
    Task ClearAsync(string chatId);
}
