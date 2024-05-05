using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class ChatMongoDBService(
    IChatRepository chatRepository,
    IMapper mapper) : IChatAgnosticService
{
    private readonly IChatRepository _chatRepository = chatRepository;
    private readonly IMapper _mapper = mapper;

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

    public async Task<ChatServiceModel> CreateIndividualChatAsync(string invitingAccountId, string invitedAccountId, CancellationToken cancellationToken = default)
    {
        var chat = await _chatRepository.CreateIndividualChatAsync([invitingAccountId, invitedAccountId], cancellationToken);

        return _mapper.Map<ChatServiceModel>(chat);
    }
}
