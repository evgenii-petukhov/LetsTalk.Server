namespace LetsTalk.Server.API.Models.Chat;

public class StartOutgoingCallRequest
{
    public string? ChatId { get; set; }

    public string? Offer { get; set; }
}
