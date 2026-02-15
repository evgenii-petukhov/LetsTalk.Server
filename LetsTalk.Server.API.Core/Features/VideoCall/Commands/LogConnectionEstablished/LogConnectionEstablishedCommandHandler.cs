using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.VideoCall.Commands.LogConnectionEstablished;

internal class LogConnectionEstablishedCommandHandler(
    ITelemetryService telemetryService) : IRequestHandler<LogConnectionEstablishedCommand, Unit>
{
    private readonly ITelemetryService _telemetryService = telemetryService;

    public async Task<Unit> Handle(LogConnectionEstablishedCommand request, CancellationToken cancellationToken)
    {
        _telemetryService.TrackConnectionEstablished(
            request.CallId,
            request.ChatId,
            request.AccountId,
            request.ConnectionDiagnostics);

        return Unit.Value;
    }
}
