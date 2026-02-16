using LetsTalk.Server.API.Logging.Models;

namespace LetsTalk.Server.API.Models.Call;

public class StartOutgoingCallRequest
{
    public string? CallId { get; set; }

    public string? ChatId { get; set; }

    public string? Offer { get; set; }

    public ConnectionDiagnostics? ConnectionDiagnostics { get; set; }

    public int IceGatheringElapsedMs { get; set; }

    public bool IceGatheringCollectedAll { get; set; }
}
