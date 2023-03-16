using LetsTalk.Server.SignalR.Abstractions;
using LetsTalk.Server.SignalR.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.SignalR;

public static class SignalrServicesRegistration
{
    public static IServiceCollection AddSignalrServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddTransient<INotificationService, NotificationService>();

        return services;
    }
}
