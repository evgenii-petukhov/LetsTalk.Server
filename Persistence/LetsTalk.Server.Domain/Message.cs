using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("messages")]
public class Message : BaseEntity
{
    public string? Text { get; protected set; }

    public string? TextHtml { get; protected set; }

    public Account? Sender { get; protected set; }

    public int SenderId { get; protected set; }

    public long? DateCreatedUnix { get; protected set; }

    public LinkPreview? LinkPreview { get; protected set; }

    public int? LinkPreviewId { get; protected set; }

    public Image? Image { get; protected set; }

    public string? ImageId { get; protected set; }

    public Image? ImagePreview { get; protected set; }

    public string? ImagePreviewId { get; protected set; }

    public Chat? Chat { get; protected set; }

    public int ChatId { get; protected set; }

    public bool EmojisOnly { get; protected set; }

    public int EmojiCount { get; protected set; }

    protected Message()
    {
    }

    public Message(int id) : base(id)
    {
    }

    public Message(
        int senderId,
        int chatId,
        string? text,
        string? textHtml,
        bool emojisOnly,
        int emojiCount,
        int? linkPreviewId = null,
        Image? image = null,
        long? dateCreatedUnix = null)
    {
        SenderId = senderId;
        ChatId = chatId;
        Text = text;
        TextHtml = textHtml;
        Image = image;
        LinkPreviewId = linkPreviewId;
        EmojisOnly = emojisOnly;
        EmojiCount = emojiCount;
        DateCreatedUnix = dateCreatedUnix;
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
}
