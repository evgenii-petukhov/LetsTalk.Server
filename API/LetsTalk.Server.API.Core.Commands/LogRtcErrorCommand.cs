using LetsTalk.Server.Telemetry.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record LogRtcErrorCommand(
    string CallId,
    string AccountId,
    string ChatId,
    ConnectionDiagnostics ConnectionDiagnostics,
    RtcErrorType ErrorType,
    string Error,
    string StackTrace) : IRequest<Unit>;
