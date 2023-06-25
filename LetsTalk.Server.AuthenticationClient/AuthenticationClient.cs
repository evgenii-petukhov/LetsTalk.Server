using Grpc.Net.Client;
using LetsTalk.Server.Authentication;
using static LetsTalk.Server.Authentication.JwtTokenGrpcService;
using LetsTalk.Server.Authentication.Abstractions;

namespace LetsTalk.Server.AuthenticationClient;

public class AuthenticationClient: IAuthenticationClient
{
    public async Task<string> GenerateJwtTokenAsync(string url, int accountId)
    {
        using var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
        {
            HttpHandler = httpHandler
        });

        var response = await new JwtTokenGrpcServiceClient(channel)
            .GenerateJwtTokenAsync(new GenerateJwtTokenRequest
            {
                AccountId = accountId
            });

        return response.Token;
    }

    public async Task<int?> ValidateJwtTokenAsync(string url, string? token)
    {
        using var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
        {
            HttpHandler = httpHandler
        });

        var response = await new JwtTokenGrpcServiceClient(channel)
            .ValidateJwtTokenAsync(new ValidateJwtTokenRequest
            {
                Token = token
            });

        return response.AccountId;
    }
}
