using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("chats")]
public class Chat : BaseEntity
{
    public Account? Sender { get; protected set; }

    public Account? Recipient { get; protected set; }

    protected Chat()
    {
    }
}
