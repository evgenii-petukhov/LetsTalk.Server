using LetsTalk.Server.Authentication;
using static LetsTalk.Server.Authentication.JwtTokenGrpcService;
using LetsTalk.Server.Authentication.Abstractions;

namespace LetsTalk.Server.AuthenticationClient;

public class AuthenticationClient: IAuthenticationClient
{
    private readonly JwtTokenGrpcServiceClient _client;

    public AuthenticationClient(JwtTokenGrpcServiceClient client)
    {
        _client = client;
    }

    public async Task<string> GenerateJwtTokenAsync(int accountId)
    {
        var response = await _client.GenerateJwtTokenAsync(new GenerateJwtTokenRequest
        {
            AccountId = accountId
        });

        return response.Token;
    }

    public async Task<int?> ValidateJwtTokenAsync(string token)
    {
        var response = await _client.ValidateJwtTokenAsync(new ValidateJwtTokenRequest
        {
            Token = token
        });

        return response.AccountId;
    }
}
