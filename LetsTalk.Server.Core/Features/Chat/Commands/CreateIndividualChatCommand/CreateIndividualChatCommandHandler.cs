using AutoMapper;
using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.Core.Features.Chat.Commands.CreateIndividualChatCommand;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateIndividualChatCommandHandler(
    IChatAgnosticService chatAgnosticService,
    IAccountAgnosticService accountAgnosticService,
    IMapper mapper) : IRequestHandler<CreateIndividualChatCommand, CreateIndividualChatResponse>
{
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;
    private readonly IMapper _mapper = mapper;

    public async Task<CreateIndividualChatResponse> Handle(CreateIndividualChatCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateIndividualChatCommandValidator(_accountAgnosticService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var chat = await _chatAgnosticService.CreateIndividualChatAsync(request.InvitingAccountId, request.AccountId, cancellationToken);

        return new CreateIndividualChatResponse
        {
            Dto = _mapper.Map<ChatDto>(chat)
        };
    }
}
