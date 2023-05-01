namespace LetsTalk.Server.API.Models.CreateMessage;

public class CreateMessageRequest
{
    public string? Text { get; set; }

    public int RecipientId { get; set; }
}
