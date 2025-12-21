using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Kafka.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Call.Commands.AcceptCall;

public class AcceptCallCommandHandler(
    IProducer<Notification> notificationProducer) : IRequestHandler<AcceptCallCommand, Unit>
{
    private readonly IProducer<Notification> _notificationProducer = notificationProducer;

    public async Task<Unit> Handle(AcceptCallCommand request, CancellationToken cancellationToken)
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
