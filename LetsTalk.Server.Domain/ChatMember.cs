using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("chatmembers")]
public class ChatMember : BaseEntity
{
    public Chat? Chat { get; protected set; }

    public Account? Account { get; protected set; }

    protected ChatMember()
    {
    }
}
