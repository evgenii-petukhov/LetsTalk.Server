using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.VideoCall.Commands.StartOutgoingCall;

public class StartOutgoingCallCommandHandler(
    IProducer<Notification> notificationProducer,
    IChatAgnosticService chatAgnosticService) : IRequestHandler<StartOutgoingCallCommand, Unit>
{
    private readonly IProducer<Notification> _notificationProducer = notificationProducer;
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;

    public async Task<Unit> Handle(StartOutgoingCallCommand request, CancellationToken cancellationToken)
    {
        var accountIds = await _chatAgnosticService.GetChatMemberAccountIdsAsync(request.ChatId, cancellationToken);

        var recipientId = accountIds.FirstOrDefault(x => x != request.AccountId);

        await _notificationProducer.PublishAsync(new Notification
        {
            RecipientId = recipientId,
            Connection = new RtcSessionSettings
            {
                Offer = request.Offer,
                ChatId = request.ChatId,
            }
        }, cancellationToken);

        return Unit.Value;
    }
}
