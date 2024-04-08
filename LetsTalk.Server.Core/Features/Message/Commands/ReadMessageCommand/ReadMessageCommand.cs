using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;

public class ReadMessageCommand : IRequest<Unit>
{
    public string? MessageId { get; set; }

    public string? AccountId { get; set; }
}