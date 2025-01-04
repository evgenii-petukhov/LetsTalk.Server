using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public class ReadMessageCommand : IRequest<Unit>
{
    public string? ChatId { get; set; }

    public string? AccountId { get; set; }

    public string? MessageId { get; set; }
}