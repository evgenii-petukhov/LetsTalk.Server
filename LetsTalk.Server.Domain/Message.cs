using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("messages")]
public class Message : BaseEntity
{
    public string? Text { get; set; }

    public string? TextHtml { get; set; }

    public Account? Sender { get; set; }

    public int SenderId { get; set; }

    public Account? Recipient { get; set; }

    public int RecipientId { get; set; }

    public bool IsRead { get; set; }

    public long? DateCreatedUnix { get; set; }

    public long? DateReadUnix { get; set; }

    public int? LinkPreviewId { get; set; }

    public LinkPreview? LinkPreview { get; set; }
}
