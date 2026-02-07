namespace LetsTalk.Server.API.Models.Chat;

public class HandleIncomingCallRequest
{
    public string? CallId { get; set; }

    public string? ChatId { get; set; }

    public string? Answer { get; set; }

    public int IceGatheringElapsedMs { get; set; }

    public bool IceGatheringCollectedAll { get; set; }
}
