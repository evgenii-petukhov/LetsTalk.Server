using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;

public class ReadMessageCommand : IRequest<Unit>
{
    public string? ChatId { get; set; }

    public string? AccountId { get; set; }

    public string? MessageId { get; set; }
}