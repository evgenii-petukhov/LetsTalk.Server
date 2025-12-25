namespace LetsTalk.Server.API.Models.Chat;

public class StartOutgoingCallRequest
{
    public string? AccountId { get; set; }

    public string? Offer { get; set; }
}
