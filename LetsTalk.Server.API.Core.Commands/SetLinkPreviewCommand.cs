using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public class SetLinkPreviewCommand : IRequest<Unit>
{
    public string? MessageId { get; set; }

    public string? ChatId { get; set; }

    public string? Url { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }
}
