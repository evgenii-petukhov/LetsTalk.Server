using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Services;
using Microsoft.Extensions.Configuration;

namespace LetsTalk.Server.Notifications;

public static class NotificationsServicesRegistration
{
    public static IServiceCollection AddNotificationsServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddTransient<INotificationService, NotificationService>();
        services.Configure<AuthenticationSettings>(configuration.GetSection("AuthenticationSettings"));

        return services;
    }
}
