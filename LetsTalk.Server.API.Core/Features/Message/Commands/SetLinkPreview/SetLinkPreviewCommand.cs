using MediatR;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.SetLinkPreview;

public class SetLinkPreviewCommand : IRequest<Unit>
{
    public string? MessageId { get; set; }

    public string? ChatId { get; set; }

    public string? Url { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }
}
