using AutoMapper;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using System.Globalization;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class ChatEntityFrameworkService(
    IChatRepository chatRepository,
    IChatMemberRepository chatMemberRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IChatAgnosticService
{
    private readonly IChatRepository _chatRepository = chatRepository;
    private readonly IChatMemberRepository _chatMemberRepository = chatMemberRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var accountIdAsInt = int.Parse(accountId, CultureInfo.InvariantCulture);
        var chats = await _chatRepository.GetChatsByAccountIdAsync(accountIdAsInt, cancellationToken);
        var chatMetrics = await _chatRepository.GetChatMetricsAsync(accountIdAsInt, cancellationToken);

        return [.. chats.Select(chat =>
        {
            chatMetrics.TryGetValue(chat.Id!, out var metrics);
            var otherAccount = chat!.ChatMembers!.FirstOrDefault(cm => cm.AccountId != accountIdAsInt)?.Account;

            return new ChatServiceModel
            {
                Id = chat.Id.ToString(CultureInfo.InvariantCulture),
                ChatName = chat.IsIndividual && otherAccount != null ? $"{otherAccount.FirstName} {otherAccount.LastName}" : chat.Name,
                PhotoUrl = chat.IsIndividual ? otherAccount?.PhotoUrl : null,
                AccountTypeId = chat.IsIndividual ? otherAccount?.AccountTypeId : null,
                Image = chat.IsIndividual && otherAccount!.Image != null ? _mapper.Map<ImageServiceModel>(otherAccount.Image) : null,
                LastMessageDate = metrics?.LastMessageDate,
                LastMessageId = metrics?.LastMessageId.ToString(CultureInfo.InvariantCulture),
                UnreadCount = metrics?.UnreadCount ?? 0,
                IsIndividual = chat.IsIndividual,
                AccountIds = [.. chat.ChatMembers!
                    .Where(cm => cm.AccountId != accountIdAsInt)
                    .Select(cm => cm.AccountId.ToString(CultureInfo.InvariantCulture))]
            };
        })];
    }

    public async Task<List<string>> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default)
    {
        var ids = await _chatMemberRepository.GetChatMemberAccountIdsAsync(int.Parse(chatId, CultureInfo.InvariantCulture), cancellationToken);

        return [.. ids.Select(x => x.ToString(CultureInfo.InvariantCulture))];
    }

    public async Task<List<string>> GetAccountIdsInIndividualChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var ids = await _chatRepository.GetAccountIdsInIndividualChatsAsync(int.Parse(accountId, CultureInfo.InvariantCulture), cancellationToken);

        return [.. ids.Select(x => x.ToString(CultureInfo.InvariantCulture))];
    }

    public Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _chatRepository.IsChatIdValidAsync(int.Parse(id, CultureInfo.InvariantCulture), cancellationToken);
    }

    public async Task<string> CreateIndividualChatAsync(
        IEnumerable<string> accountIds,
        CancellationToken cancellationToken = default)
    {
        var accountIdsAsInt = accountIds
            .Select(int.Parse)
            .ToList();

        var chat = await _chatRepository.GetIndividualChatByAccountIdsAsync(accountIdsAsInt, cancellationToken);

        if (chat == null)
        {
            chat = new Chat(accountIdsAsInt);

            await _chatRepository.CreateAsync(chat, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
        }

        return chat.Id.ToString(CultureInfo.InvariantCulture);
    }
}
