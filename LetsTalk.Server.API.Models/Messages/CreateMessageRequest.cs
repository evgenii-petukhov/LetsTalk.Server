namespace LetsTalk.Server.API.Models.Messages;

public class CreateMessageRequest
{
    public string? Text { get; set; }

    public string? RecipientId { get; set; }

    public int? ImageId { get; set; }
}
