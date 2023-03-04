namespace LetsTalk.Server.Domain;

public class Message: BaseEntity
{
    public string? Text { get; set; }

    public Account? Sender { get; set; }

    public int SenderId { get; set; }

    public Account? Recipient { get; set; }

    public int RecipientId { get; set; }

    public bool IsRead { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateRead { get; set; }
}
