using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record HandleIncomingCallCommand(
    string CallId,
    string AccountId,
    string ChatId,
    string Answer,
    string ConnectionState,
    string LocalCandidateTypes,
    string RemoteCandidateTypes,
    string Browser,
    string Platform,
    int IceGatheringElapsedMs,
    bool IceGatheringCollectedAll) : IRequest<Unit>;
