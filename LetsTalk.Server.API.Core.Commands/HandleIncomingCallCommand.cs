using LetsTalk.Server.API.Core.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record HandleIncomingCallCommand(
    string CallId,
    string AccountId,
    string ChatId,
    string Answer,
    int IceGatheringElapsedMs,
    bool IceGatheringCollectedAll,
    ConnectionDiagnostics ConnectionDiagnostics) : IRequest<Unit>;
