using LetsTalk.Server.API.Logging.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record LogConnectionEstablishedCommand(
    string CallId,
    string AccountId,
    string ChatId,
    ConnectionDiagnostics ConnectionDiagnostics) : IRequest<Unit>;
