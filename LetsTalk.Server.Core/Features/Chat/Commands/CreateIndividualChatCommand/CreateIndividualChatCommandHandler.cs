using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.Core.Features.Chat.Commands.CreateIndividualChatCommand;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateIndividualChatCommandHandler : IRequestHandler<CreateIndividualChatCommand, CreateIndividualChatResponse>
{
    private readonly IChatAgnosticService _chatAgnosticService;

    public CreateIndividualChatCommandHandler(IChatAgnosticService chatAgnosticService)
    {
        _chatAgnosticService = chatAgnosticService;
    }

    public Task<CreateIndividualChatResponse> Handle(CreateIndividualChatCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CreateIndividualChatResponse
        {
            Dto = new ChatDto
            {
                Id = "100"
            }
        });
    }
}
