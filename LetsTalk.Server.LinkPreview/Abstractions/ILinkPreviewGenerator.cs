namespace LetsTalk.Server.LinkPreview.Abstractions;

public interface ILinkPreviewGenerator
{
    Task<Domain.LinkPreview?> GetLinkPreviewAsync(string url);
}
