namespace LetsTalk.Server.Notifications.Abstractions;

public interface INotificationService
{
    Task SendNotificationAsync<T>(int accountId, T notification, string typeName);
}