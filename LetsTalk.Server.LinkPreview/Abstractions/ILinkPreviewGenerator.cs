namespace LetsTalk.Server.LinkPreview.Abstractions;

public interface ILinkPreviewGenerator
{
    Task SetLinkPreviewAsync(int messageId, string url);
}
