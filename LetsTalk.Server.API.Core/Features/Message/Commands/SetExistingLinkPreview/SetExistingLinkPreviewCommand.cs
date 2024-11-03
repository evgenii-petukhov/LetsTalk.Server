using MediatR;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.SetExistingLinkPreview;

public class SetExistingLinkPreviewCommand : IRequest<Unit>
{
    public string? MessageId { get; set; }

    public string? ChatId { get; set; }

    public string? Url { get; set; }
}
