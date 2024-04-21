using LetsTalk.Server.Persistence.Utility;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("chatmessagestatuses")]
[PrimaryKey("ChatId", "AccountId", "MessageId")]
public class ChatMessageStatus
{
    public Chat? Chat { get; protected set; }

    public int ChatId { get; protected set; }

    public Account? Account { get; protected set; }

    public int AccountId { get; protected set; }

    public Message? Message { get; protected set; }

    public int MessageId { get; protected set; }

    public long DateReadUnix { get; protected set; }

    protected ChatMessageStatus()
    {
    }

    public ChatMessageStatus(int chatId, int accountId, int messageId)
    {
        ChatId = chatId;
        AccountId = accountId;
        MessageId = messageId;
    }

    public void MarkAsRead()
    {
        DateReadUnix = DateHelper.GetUnixTimestamp();
    }
}
