namespace LetsTalk.Server.LinkPreview.Abstractions;

public interface ILinkPreviewGenerator
{
    Task ProcessMessageAsync(string messageId, string chatId, string url);
}
