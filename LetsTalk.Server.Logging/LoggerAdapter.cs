using LetsTalk.Server.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Server.Logging;

public class LoggerAdapter<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<T>();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "<Pending>")]
    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "<Pending>")]
    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }
}
