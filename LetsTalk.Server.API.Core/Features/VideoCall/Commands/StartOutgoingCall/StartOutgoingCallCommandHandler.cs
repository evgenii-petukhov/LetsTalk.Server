using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Telemetry.Abstractions;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.VideoCall.Commands.StartOutgoingCall;

public class StartOutgoingCallCommandHandler(
    IProducer<Notification> notificationProducer,
    IChatAgnosticService chatAgnosticService,
    ITelemetryService telemetryService) : IRequestHandler<StartOutgoingCallCommand, StartOutgoingCallDto>
{
    private readonly IProducer<Notification> _notificationProducer = notificationProducer;
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly ITelemetryService _telemetryService = telemetryService;

    public async Task<StartOutgoingCallDto> Handle(StartOutgoingCallCommand request, CancellationToken cancellationToken)
    {
        var accountIds = await _chatAgnosticService.GetChatMemberAccountIdsAsync(request.ChatId, cancellationToken);

        var recipientId = accountIds.FirstOrDefault(x => x != request.AccountId);

        var callId = Guid.NewGuid().ToString();

        await _notificationProducer.PublishAsync(new Notification
        {
            RecipientId = recipientId,
            IncomingCall = new IncomingCallRequest
            {
                CallId = callId,
                Offer = request.Offer,
                ChatId = request.ChatId,
            }
        }, cancellationToken);

        _telemetryService.TrackStartOutgoingCall(
            callId,
            request.ChatId,
            request.AccountId,
            request.IceGatheringElapsedMs,
            request.IceGatheringCollectedAll,
            request.ConnectionDiagnostics);

        return new StartOutgoingCallDto
        {
            CallId = callId
        };
    }
}
