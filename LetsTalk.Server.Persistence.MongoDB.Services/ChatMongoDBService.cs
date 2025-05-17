using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class ChatMongoDBService(
    IChatRepository chatRepository) : IChatAgnosticService
{
    private readonly IChatRepository _chatRepository = chatRepository;

    public Task<List<string>> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default)
    {
        return _chatRepository.GetChatMemberAccountIdsAsync(chatId, cancellationToken);
    }

    public async Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var chats = await _chatRepository.GetChatsByAccountIdAsync(accountId, cancellationToken);

        var accounts = await _chatRepository.GetAccountsByChatsAsync(chats, accountId, cancellationToken);

        var chatMetrics = _chatRepository.GetChatMetrics(accountId, cancellationToken);

        return chats.Select(chat =>
        {
            chatMetrics.TryGetValue(chat.Id!, out var metrics);
            var otherAccount = accounts.FirstOrDefault(a => chat.AccountIds!.Contains(a.Id!));

            return new ChatServiceModel
            {
                Id = chat.Id,
                ChatName = chat.IsIndividual ? $"{otherAccount?.FirstName} {otherAccount?.LastName}" : chat.Name,
                PhotoUrl = chat.IsIndividual ? otherAccount?.PhotoUrl : null,
                AccountTypeId = chat.IsIndividual ? otherAccount?.AccountTypeId : null,
                Image = chat.IsIndividual && otherAccount?.Image != null ? new ImageServiceModel
                {
                    Id = otherAccount.Image.Id,
                    FileStorageTypeId = otherAccount.Image.FileStorageTypeId
                } : null,
                LastMessageDate = metrics?.LastMessageDate,
                LastMessageId = metrics?.LastMessageId,
                UnreadCount = metrics?.UnreadCount ?? 0,
                IsIndividual = chat.IsIndividual,
                AccountIds = chat.AccountIds!.Where(x => x != accountId).ToList()
            };
        }).ToList();
    }

    public Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _chatRepository.IsChatIdValidAsync(id, cancellationToken);
    }

    public async Task<string> CreateIndividualChatAsync(IEnumerable<string> accountIds, CancellationToken cancellationToken = default)
    {
        var chat = await _chatRepository.GetIndividualChatByAccountIdsAsync(accountIds, cancellationToken);

        chat ??= await _chatRepository.CreateIndividualChatAsync(accountIds, cancellationToken);

        return chat.Id!;
    }

    public Task<List<string>> GetAccountIdsInIndividualChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        return _chatRepository.GetAccountIdsInIndividualChatsAsync(accountId, cancellationToken);
    }
}
