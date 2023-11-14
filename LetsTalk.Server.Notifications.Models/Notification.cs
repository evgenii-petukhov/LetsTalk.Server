namespace LetsTalk.Server.Notifications.Models;

public class Notification<T> where T : class
{
    public string? RecipientId { get; set; }

    public T? Message { get; set; }
}
