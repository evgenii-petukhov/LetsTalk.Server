using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Telemetry.Abstractions;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.VideoCall.Commands.HandleIncomingCall;

public class HandleIncomingCallCommandHandler(
    IProducer<Notification> notificationProducer,
    IChatAgnosticService chatAgnosticService,
    ITelemetryService telemetryService) : IRequestHandler<HandleIncomingCallCommand, Unit>
{
    private readonly IProducer<Notification> _notificationProducer = notificationProducer;
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly ITelemetryService _telemetryService = telemetryService;

    public async Task<Unit> Handle(HandleIncomingCallCommand request, CancellationToken cancellationToken)
    {
        var accountIds = await _chatAgnosticService.GetChatMemberAccountIdsAsync(request.ChatId, cancellationToken);

        var recipientId = accountIds.FirstOrDefault(x => x != request.AccountId);

        await _notificationProducer.PublishAsync(new Notification
        {
            RecipientId = recipientId,
            Connection = new RtcSessionSettings
            {
                CallId = request.CallId,
                ChatId = request.ChatId,
                Answer = request.Answer
            }
        }, cancellationToken);

        _telemetryService.TrackHandleIncomingCall(
            request.CallId,
            request.ChatId,
            request.AccountId,
            request.IceGatheringElapsedMs,
            request.IceGatheringCollectedAll,
            request.ConnectionDiagnostics);

        return Unit.Value;
    }
}
