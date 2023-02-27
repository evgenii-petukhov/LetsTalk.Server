namespace LetsTalk.Server.Models.Message;

public class MessageDto
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public int SenderId { get; set; }

    public int RecipientId { get; set; }

    public DateTime Created { get; set; }
}
