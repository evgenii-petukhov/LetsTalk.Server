namespace LetsTalk.Server.API.Core.Abstractions;

public interface IImageProcessingLauncher
{
    Task LaunchAsync(
        string messageId,
        string imageId,
        string chatId,
        int fileStorageTypeId,
        string token,
        CancellationToken cancellationToken = default);
}
