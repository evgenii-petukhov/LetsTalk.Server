using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;

public class ReadMessageCommand : IRequest<Unit>
{
    public int MessageId { get; set; }

    public string? RecipientId { get; set; }

    public bool UpdatePreviousMessages { get; set; }
}