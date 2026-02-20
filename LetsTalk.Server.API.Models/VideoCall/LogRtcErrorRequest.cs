using LetsTalk.Server.Telemetry.Models;

namespace LetsTalk.Server.API.Models.VideoCall;

public class LogRtcErrorRequest
{
    public string? CallId { get; set; }

    public string? ChatId { get; set; }

    public ConnectionDiagnostics? ConnectionDiagnostics { get; set; }

    public RtcErrorType ErrorType { get; set; }

    public string? Error { get; set; }

    public string? StackTrace { get; set; }
}
