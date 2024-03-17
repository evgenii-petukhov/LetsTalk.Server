using LetsTalk.Server.Persistence.Utility;
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

    public LinkPreview? LinkPreview { get; protected set; }

    public Image? Image { get; protected set; }

    public string? ImageId { get; protected set; }

    public Image? ImagePreview { get; protected set; }

    public string? ImagePreviewId { get; protected set; }

    public Chat? Chat { get; protected set; }

    protected Message()
    {
    }

    public Message(int id) : base(id)
    {
    }

    public Message(int senderId, int recipientId, string? text, string? textHtml, Image? image = null)
    {
        SenderId = senderId;
        RecipientId = recipientId;
        Text = text;
        TextHtml = textHtml;
        Image = image;
    }

    public void SetImagePreview(Image image)
    {
        ImagePreview = image;
    }

    public void SetLinkPreview(LinkPreview linkPreview)
    {
        LinkPreview = linkPreview;
    }

    public void SetDateCreatedUnix(long? dateCreatedUnix)
    {
        DateCreatedUnix = dateCreatedUnix;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        DateReadUnix = DateHelper.GetUnixTimestamp();
    }
}
