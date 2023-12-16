using LetsTalk.Server.API.Models.Messages;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommand : IRequest<CreateMessageResponse>
{
    public string? SenderId { get; set; }

    public string? RecipientId { get; set; }

    public string? Text { get; set; }

    public string? ImageId { get; set; }
}