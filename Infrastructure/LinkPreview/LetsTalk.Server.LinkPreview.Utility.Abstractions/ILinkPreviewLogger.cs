namespace LetsTalk.Server.LinkPreview.Utility.Abstractions;

public interface ILinkPreviewLogger
{
    void LogInformation(string message);

    void LogError(Exception exception, string message);
}
