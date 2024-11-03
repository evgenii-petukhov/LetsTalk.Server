using LetsTalk.Server.SignPackage.Models;

namespace LetsTalk.Server.API.Models.Message;

public class SetLinkPreviewRequest : ISignable
{
    public string? MessageId { get; set; }

    public string? ChatId { get; set; }

    public string? Url { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }

    public string? Signature { get; set; }
}
