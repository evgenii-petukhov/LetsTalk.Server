using LetsTalk.Server.API.Models.CreateMessage;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommand : IRequest<CreateMessageResponse>
{
    public int? SenderId { get; set; }

    public int? RecipientId { get; set; }

    public string? Text { get; set; }
}