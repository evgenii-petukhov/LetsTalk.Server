namespace LetsTalk.Server.API.Models.Messages;

public class CreateMessageRequest
{
    public string? Text { get; set; }

    public string? ChatId { get; set; }

    public ImageRequestModel? Image { get; set; }
}
