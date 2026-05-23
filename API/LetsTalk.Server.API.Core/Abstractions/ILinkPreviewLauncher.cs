namespace LetsTalk.Server.API.Core.Abstractions;

public interface ILinkPreviewLauncher
{
    Task LaunchAsync(
        string messageId,
        string url,
        string chatId,
        string token,
        CancellationToken cancellationToken);
}
