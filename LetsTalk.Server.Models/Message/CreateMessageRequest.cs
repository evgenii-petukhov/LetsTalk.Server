namespace LetsTalk.Server.Models.Message;

public class CreateMessageRequest
{
    public string? Text { get; set; }

    public int RecipientId { get; set; }
}
