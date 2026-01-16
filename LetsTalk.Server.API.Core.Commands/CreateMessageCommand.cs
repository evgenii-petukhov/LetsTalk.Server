using LetsTalk.Server.API.Models.Message;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public class CreateMessageCommand : IRequest<CreateMessageResponse>
{
    public string? SenderId { get; set; }

    public string? ChatId { get; set; }

    public string? Text { get; set; }

    public ImageRequestModel? Image { get; set; }

    public string? Token { get; set; }
}