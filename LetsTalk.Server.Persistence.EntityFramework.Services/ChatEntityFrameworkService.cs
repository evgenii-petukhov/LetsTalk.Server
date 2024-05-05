using AutoMapper;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

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

    public Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        return  _chatRepository.GetChatsAsync(int.Parse(accountId), cancellationToken);
    }

    public async Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default)
    {
        var ids = await _chatMemberRepository.GetChatMemberAccountIdsAsync(int.Parse(chatId), cancellationToken);

        return ids.Select(x => x.ToString()).ToArray();
    }

    public Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _chatRepository.IsChatIdValidAsync(int.Parse(id), cancellationToken);
    }

    public async Task<ChatServiceModel> CreateIndividualChatAsync(string[] accountIds, CancellationToken cancellationToken = default)
    {
        var chat = new Chat(accountIds.Select(int.Parse));

        await _chatRepository.CreateAsync(chat, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<ChatServiceModel>(chat);
    }
}
