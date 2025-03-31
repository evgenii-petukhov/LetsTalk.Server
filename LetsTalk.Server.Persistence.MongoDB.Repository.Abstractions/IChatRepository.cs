﻿using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions.Models;
namespace LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

public interface IChatRepository
{
    Task<List<Chat>> GetChatsByAccountIdAsync(string accountId, CancellationToken cancellationToken = default);

    Task<List<Account>> GetAccountsByChatsAsync(IEnumerable<Chat> chats, string accountId, CancellationToken cancellationToken = default);

    Dictionary<string, ChatMetric> GetChatMetrics(string accountId, CancellationToken cancellationToken = default);

    Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default);

    Task<Chat> GetIndividualChatByAccountIdsAsync(string[] accountIds, CancellationToken cancellationToken = default);

    Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default);

    Task<Chat> CreateIndividualChatAsync(string[] accountIds, CancellationToken cancellationToken = default);
}
