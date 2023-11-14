namespace LetsTalk.Server.Notifications.Abstractions;

public interface INotificationService
{
    Task SendNotificationAsync<T>(string accountId, T notification, string typeName);
}