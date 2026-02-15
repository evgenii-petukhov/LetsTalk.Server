using LetsTalk.Server.API.Core.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record LogConnectionFailedCommand(
    string CallId,
    string AccountId,
    string ChatId,
    ConnectionDiagnostics ConnectionDiagnostics,
    string error,
    string StackTrace) : IRequest<Unit>;
