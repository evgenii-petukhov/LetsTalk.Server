using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("messages")]
public class Message : BaseEntity
{
    public string? Text { get; protected set; }

    public string? TextHtml { get; protected set; }

    public Account? Sender { get; protected set; }

    public int SenderId { get; protected set; }

    public Account? Recipient { get; protected set; }

    public int RecipientId { get; protected set; }

    public bool IsRead { get; protected set; }

    public long? DateCreatedUnix { get; protected set; }

    public long? DateReadUnix { get; protected set; }

    public int? LinkPreviewId { get; protected set; }

    public LinkPreview? LinkPreview { get; protected set; }

    public Image? Image { get; protected set; }

    public int? ImageId { get; protected set; }

    public Image? ImagePreview { get; protected set; }

    public int? ImagePreviewId { get; protected set; }

    protected Message()
    {

    }

    public void SetImagePreview(Image image)
    {
        ImagePreview = image;
    }

    public void SetTextHtml(string? html)
    {
        TextHtml = html;
    }

    public void SetDateCreatedUnix(long? dateCreatedUnix)
    {
        DateCreatedUnix = dateCreatedUnix;
    }
}
