using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record HandleIncomingCallCommand(
    string AccountId,
    string Answer) : IRequest<Unit>;
