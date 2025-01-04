using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using System.Globalization;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class ChatEntityFrameworkService(
    IChatRepository chatRepository,
    IChatMemberRepository chatMemberRepository,
    IUnitOfWork unitOfWork) : IChatAgnosticService
{
    private readonly IChatRepository _chatRepository = chatRepository;
    private readonly IChatMemberRepository _chatMemberRepository = chatMemberRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        return  _chatRepository.GetChatsAsync(int.Parse(accountId, CultureInfo.InvariantCulture), cancellationToken);
    }

    public async Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default)
    {
        var ids = await _chatMemberRepository.GetChatMemberAccountIdsAsync(int.Parse(chatId, CultureInfo.InvariantCulture), cancellationToken);

        return ids.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
    }

    public Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _chatRepository.IsChatIdValidAsync(int.Parse(id, CultureInfo.InvariantCulture), cancellationToken);
    }

    public async Task<string> CreateIndividualChatAsync(
        string[] accountIds,
        CancellationToken cancellationToken = default)
    {
        var accountIdsAsInt = accountIds
            .Select(int.Parse)
            .ToArray();

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
