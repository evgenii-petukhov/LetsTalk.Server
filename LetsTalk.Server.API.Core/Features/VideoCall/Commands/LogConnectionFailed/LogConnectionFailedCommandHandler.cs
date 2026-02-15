using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.VideoCall.Commands.LogConnectionEstablished;

internal class LogConnectionFailedCommandHandler(
    ITelemetryService telemetryService) : IRequestHandler<LogConnectionFailedCommand, Unit>
{
    private readonly ITelemetryService _telemetryService = telemetryService;

    public async Task<Unit> Handle(LogConnectionFailedCommand request, CancellationToken cancellationToken)
    {
        _telemetryService.TrackConnectionFailed(
            request.CallId,
            request.ChatId,
            request.AccountId,
            request.ConnectionDiagnostics);

        return Unit.Value;
    }
}
