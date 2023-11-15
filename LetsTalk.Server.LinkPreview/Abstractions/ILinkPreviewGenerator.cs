using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.LinkPreview.Abstractions;

public interface ILinkPreviewGenerator
{
    Task<MessageServiceModel?> ProcessMessageAsync(string messageId, string url);
}
