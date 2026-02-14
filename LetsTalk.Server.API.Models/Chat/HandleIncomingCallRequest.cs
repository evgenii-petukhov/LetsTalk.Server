namespace LetsTalk.Server.API.Models.Chat;

public class HandleIncomingCallRequest
{
    public string? CallId { get; set; }

    public string? ChatId { get; set; }

    public string? Answer { get; set; }

    public string? ConnectionState { get; set; }

    public string? LocalCandidateTypes { get; set; }

    public string? RemoteCandidateTypes { get; set; }

    public string? Browser { get; set; }

    public string? Platform { get; set; }

    public int IceGatheringElapsedMs { get; set; }

    public bool IceGatheringCollectedAll { get; set; }
}
