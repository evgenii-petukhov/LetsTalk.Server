using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Kafka.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.VideoCall.Commands.StartOutgoingCall;

public class StartOutgoingCallCommandHandler(
    IProducer<Notification> notificationProducer) : IRequestHandler<StartOutgoingCallCommand, Unit>
{
    private readonly IProducer<Notification> _notificationProducer = notificationProducer;

    public async Task<Unit> Handle(StartOutgoingCallCommand request, CancellationToken cancellationToken)
    {
        await _notificationProducer.PublishAsync(new Notification
        {
            RecipientId = request.AccountId,
            Connection = new RtcSessionSettings
            {
                Offer = request.Offer,
                AccountId = request.InvitingAccountId,
            }
        }, cancellationToken);

        return Unit.Value;
    }
}
