using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.SignalR.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.SignalR;

public static class SignalrServicesRegistration
{
    public static IServiceCollection AddSignalrServices(this IServiceCollection services)
    {
        services.AddSingleton<IMessageHubConnectionManager, MessageHubConnectionManager>();

        return services;
    }
}
