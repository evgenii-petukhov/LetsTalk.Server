using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewLogger(ILogger<LinkPreviewLogger> logger) : ILinkPreviewLogger
{
    private static readonly Action<ILogger, string, Exception?> _logError =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(0, nameof(LinkPreviewLogger)),
            "{Message}");

    private static readonly Action<ILogger, string, Exception?> _logInformation =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(LinkPreviewLogger)),
            "{Message}");

    private readonly ILogger<LinkPreviewLogger> _logger = logger;

    public void LogError(Exception exception, string message)
    {
        _logError(_logger, message, exception);
    }

    public void LogInformation(string message)
    {
        _logInformation(_logger, message, null);
    }
}
