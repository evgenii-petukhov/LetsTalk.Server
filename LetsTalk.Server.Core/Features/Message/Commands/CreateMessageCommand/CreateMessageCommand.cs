using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommand: IRequest<MessageDto>
{
    public int SenderId { get; set; }

    public int RecipientId { get; set; }

    public string? Text { get; set; }
}