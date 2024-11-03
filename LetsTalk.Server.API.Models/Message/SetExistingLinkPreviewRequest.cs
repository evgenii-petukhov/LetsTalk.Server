using LetsTalk.Server.SignPackage.Models;

namespace LetsTalk.Server.API.Models.Message;

public class SetExistingLinkPreviewRequest : ISignable
{
    public string? MessageId { get; set; }

    public string? ChatId { get; set; }

    public string? Url { get; set; }

    public string? Signature { get; set; }
}
