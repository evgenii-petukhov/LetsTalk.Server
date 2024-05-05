using AutoMapper;
using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.Core.Features.Chat.Commands.CreateIndividualChatCommand;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateIndividualChatCommandHandler(
    IChatAgnosticService chatAgnosticService,
    IMapper mapper) : IRequestHandler<CreateIndividualChatCommand, CreateIndividualChatResponse>
{
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly IMapper _mapper = mapper;

    public async Task<CreateIndividualChatResponse> Handle(CreateIndividualChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _chatAgnosticService.CreateIndividualChatAsync(request.InvitingAccountId, request.InvitedAccountId, cancellationToken);

        return new CreateIndividualChatResponse
        {
            Dto = _mapper.Map<ChatDto>(chat)
        };
    }
}
