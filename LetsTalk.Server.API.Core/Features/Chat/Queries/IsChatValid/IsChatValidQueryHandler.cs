using LetsTalk.Server.API.Core.Abstractions;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Chat.Queries.IsChatValid;

public class IsChatValidQueryHandler(
    IChatService chatService) : IRequestHandler<IsChatValidQuery, bool>
{
    private readonly IChatService _chatService = chatService;

    public Task<bool> Handle(IsChatValidQuery request, CancellationToken cancellationToken)
    {
        return _chatService.IsChatIdValidAsync(request.Id, cancellationToken);
    }
}
