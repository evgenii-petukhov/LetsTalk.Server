using LetsTalk.Server.Authentication.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LetsTalk.Server.Authentication.JwtTokenGrpcService;

namespace LetsTalk.Server.AuthenticationClient;

public static class AuthenticationClientServicesRegistration
{
    public static IServiceCollection AddAuthenticationClientServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAuthenticationClient, AuthenticationClient>();
        services.AddGrpcClient<JwtTokenGrpcServiceClient>(options => options.Address = new Uri(configuration.GetValue<string>("ApplicationUrls:Authentication")!)).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
#if DEBUG
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
#endif
        });

        return services;
    }
}
