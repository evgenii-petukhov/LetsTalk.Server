using LetsTalk.Server.API.Core.Models;

namespace LetsTalk.Server.API.Models.Call;

public class LogConnectionEstablishedRequest
{
    public string? CallId { get; set; }

    public string? ChatId { get; set; }

    public ConnectionDiagnostics? ConnectionDiagnostics { get; set; }
}
