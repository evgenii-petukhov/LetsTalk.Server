using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record InitializeCallCommand(
    string AccountId,
    string Offer) : IRequest<Unit>;
