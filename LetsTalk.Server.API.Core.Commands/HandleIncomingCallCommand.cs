using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record HandleIncomingCallCommand(
    string CallId,
    string AccountId,
    string ChatId,
    string Answer) : IRequest<Unit>;
