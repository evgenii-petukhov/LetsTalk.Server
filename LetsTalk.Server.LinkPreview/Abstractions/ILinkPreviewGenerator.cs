using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.LinkPreview.Abstractions;

public interface ILinkPreviewGenerator
{
    Task<MessageAgnosticModel?> ProcessMessageAsync(int messageId, string url);
}
