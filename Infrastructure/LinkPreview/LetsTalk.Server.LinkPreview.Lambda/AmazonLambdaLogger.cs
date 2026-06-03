using Amazon.Lambda.Core;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;

namespace LetsTalk.Server.LinkPreview.Lambda;

public class AmazonLambdaLogger(ILambdaLogger lambdaLogger) : ILinkPreviewLogger
{
    private readonly ILambdaLogger _lambdaLogger = lambdaLogger;

    public void LogError(Exception exception, string message)
    {
        _lambdaLogger.LogError(exception, message);
    }

    public void LogInformation(string message)
    {
        _lambdaLogger.LogInformation(message);
    }
}
