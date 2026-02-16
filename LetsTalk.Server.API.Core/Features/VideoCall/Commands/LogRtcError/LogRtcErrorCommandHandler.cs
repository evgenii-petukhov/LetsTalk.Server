using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.VideoCall.Commands.LogRtcError;

internal class LogRtcErrorCommandHandler(
    ITelemetryService telemetryService) : IRequestHandler<LogRtcErrorCommand, Unit>
{
    private readonly ITelemetryService _telemetryService = telemetryService;

    public async Task<Unit> Handle(LogRtcErrorCommand request, CancellationToken cancellationToken)
    {
        _telemetryService.TrackConnectionFailed(
            request.CallId,
            request.ChatId,
            request.AccountId,
            request.ConnectionDiagnostics,
            request.ErrorType,
            request.Error,
            request.StackTrace);

        return Unit.Value;
    }
}
