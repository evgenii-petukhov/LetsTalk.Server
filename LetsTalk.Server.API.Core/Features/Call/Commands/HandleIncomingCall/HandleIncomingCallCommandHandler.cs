using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Kafka.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Call.Commands.HandleIncomingCall;

public class HandleIncomingCallCommandHandler(
    IProducer<Notification> notificationProducer) : IRequestHandler<HandleIncomingCallCommand, Unit>
{
    private readonly IProducer<Notification> _notificationProducer = notificationProducer;

    public async Task<Unit> Handle(HandleIncomingCallCommand request, CancellationToken cancellationToken)
    {
        await _notificationProducer.PublishAsync(new Notification
        {
            RecipientId = request.AccountId,
            Connection = new RtcSessionSettings
            {
                Answer = request.Answer
            }
        }, cancellationToken);

        return Unit.Value;
    }
}
