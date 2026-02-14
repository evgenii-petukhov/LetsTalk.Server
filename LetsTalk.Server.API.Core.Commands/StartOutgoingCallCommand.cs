using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record StartOutgoingCallCommand(
    string AccountId,
    string ChatId,
    string Offer,
    string ConnectionState,
    string LocalCandidateTypes,
    string RemoteCandidateTypes,
    string Browser,
    string Platform,
    int IceGatheringElapsedMs,
    bool IceGatheringCollectedAll) : IRequest<Unit>;
