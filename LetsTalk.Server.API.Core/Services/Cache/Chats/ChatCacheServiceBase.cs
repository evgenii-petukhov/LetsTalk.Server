using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.Chats;

public abstract class ChatCacheServiceBase
{
    protected bool IsActive { get; }

    protected bool IsVolatile { get; }

    protected TimeSpan CacheLifeTimeInSeconds { get; }

    protected IChatService ChatService { get; }

    protected ChatCacheServiceBase(
        IChatService chatService,
        IOptions<CachingSettings> cachingSettings)
    {
        ChatService = chatService;

        IsActive = cachingSettings.Value.ChatCacheLifeTimeInSeconds != 0;
        IsVolatile = IsActive && cachingSettings.Value.ChatCacheLifeTimeInSeconds > 0;

        if (IsVolatile)
        {
            CacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ChatCacheLifeTimeInSeconds);
        }
    }

    protected static string GetChatsKey(string accountId)
    {
        return $"chats:{accountId}";
    }

    public Task<bool> IsChatIdValidAsync(string chatId, CancellationToken cancellationToken)
    {
        return ChatService.IsChatIdValidAsync(chatId, cancellationToken);
    }

    public Task<bool> IsAccountChatMemberAsync(string chatId, string accountId, CancellationToken cancellationToken = default)
    {
        return ChatService.IsAccountChatMemberAsync(chatId, accountId, cancellationToken);
    }
}
