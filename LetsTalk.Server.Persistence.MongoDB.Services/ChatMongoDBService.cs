﻿using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class ChatMongoDBService(
    IChatRepository chatRepository) : IChatAgnosticService
{
    private readonly IChatRepository _chatRepository = chatRepository;

    public Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default)
    {
        return _chatRepository.GetChatMemberAccountIdsAsync(chatId, cancellationToken);
    }

    public Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        return _chatRepository.GetChatsAsync(accountId, cancellationToken);
    }

    public Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _chatRepository.IsChatIdValidAsync(id, cancellationToken);
    }

    public async Task<string> CreateIndividualChatAsync(string[] accountIds, CancellationToken cancellationToken = default)
    {
        var chat = await _chatRepository.GetIndividualChatByAccountIdsAsync(accountIds, cancellationToken);

        chat ??= await _chatRepository.CreateIndividualChatAsync(accountIds, cancellationToken);

        return chat.Id!;
    }
}
