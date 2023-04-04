using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Services;
using System.Reflection;

namespace LetsTalk.Server.Notifications;

public static class NotificationsServicesRegistration
{
    public static IServiceCollection AddNotificationsServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
