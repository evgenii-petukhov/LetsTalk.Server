using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record StartOutgoingCallCommand(
    string InvitingAccountId,
    string AccountId,
    string Offer) : IRequest<Unit>;
