using LetsTalk.Server.Telemetry.Models;
using LetsTalk.Server.Models.Dtos;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record StartOutgoingCallCommand(
    string AccountId,
    string ChatId,
    string Offer,
    int IceGatheringElapsedMs,
    bool IceGatheringCollectedAll,
    ConnectionDiagnostics ConnectionDiagnostics) : IRequest<StartOutgoingCallDto>;
