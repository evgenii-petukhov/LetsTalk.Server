using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record StartOutgoingCallCommand(
    string AccountId,
    string ChatId,
    string Offer) : IRequest<Unit>;
