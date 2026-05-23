namespace LetsTalk.Server.Notifications.Abstractions;

public interface INotificationHub
{
    Task SendNotificationAsync<T>(T notification, string typeName);
}