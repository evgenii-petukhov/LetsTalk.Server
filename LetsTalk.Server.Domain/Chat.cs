using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("chats")]
public class Chat : BaseEntity
{
    public string? Name { get; protected set; }

    public Image? Image { get; protected set; }

    public string? ImageId { get; protected set; }

    public bool IsIndividual { get; protected set; }

    protected Chat()
    {
    }
}
