namespace LetsTalk.Server.API.Logging.Models;

public class ConnectionDiagnostics
{
    public string? ConnectionState { get; set; }

    public string? LocalCandidateTypes { get; set; }

    public string? RemoteCandidateTypes { get; set; }

    public string? Browser { get; set; }

    public string? Platform { get; set; }
}
