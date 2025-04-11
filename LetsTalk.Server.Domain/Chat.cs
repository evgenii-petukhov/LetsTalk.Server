using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("chats")]
public class Chat : BaseEntity
{
    public string? Name { get; protected set; }

    public Image? Image { get; protected set; }

    public string? ImageId { get; protected set; }

    public bool IsIndividual { get; protected set; }

    public ICollection<ChatMember>? ChatMembers { get; protected set; }

    public ICollection<Message>? Messages { get; protected set; }

    protected Chat()
    {
    }

    public Chat(IEnumerable<int> accountIds)
    {
        IsIndividual = true;

        ChatMembers = accountIds
            .Select(accountId => new ChatMember(accountId))
            .ToList();
    }
}
