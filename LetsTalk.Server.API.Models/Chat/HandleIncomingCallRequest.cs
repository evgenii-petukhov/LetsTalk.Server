namespace LetsTalk.Server.API.Models.Chat;

public class HandleIncomingCallRequest
{
    public string? CallId { get; set; }

    public string? ChatId { get; set; }

    public string? Answer { get; set; }
}
