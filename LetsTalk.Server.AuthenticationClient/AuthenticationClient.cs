using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using LetsTalk.Server.Authentication;
using static LetsTalk.Server.Authentication.JwtTokenGrpcService;
using LetsTalk.Server.Authentication.Abstractions;

namespace LetsTalk.Server.AuthenticationClient;

public class AuthenticationClient: IAuthenticationClient
{
    public async Task<string> GenerateJwtToken(string url, int accountId)
    {
        var response = await GetGrpcClient(url)
            .GenerateJwtTokenAsync(new GenerateJwtTokenRequest
            {
                AccountId = accountId
            });

        return response.Token;
    }

    public async Task<int?> ValidateJwtToken(string url, string? token)
    {
        var response = await GetGrpcClient(url)
            .ValidateJwtTokenAsync(new ValidateJwtTokenRequest
            {
                Token = token
            });

        return response.AccountId;
    }

    private static JwtTokenGrpcServiceClient GetGrpcClient(string url)
    {
        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
        {
            HttpHandler = new GrpcWebHandler(httpHandler)
        });

        return new JwtTokenGrpcServiceClient(channel);
    }
}
