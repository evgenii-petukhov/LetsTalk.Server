using LetsTalk.Server.Authentication;
using LetsTalk.Server.Authentication.Abstractions;
using static LetsTalk.Server.Authentication.JwtTokenGrpcService;
using static LetsTalk.Server.Authentication.LoginCodeGrpcService;

namespace LetsTalk.Server.AuthenticationClient;

public class AuthenticationClient(
    JwtTokenGrpcServiceClient tokenClient,
    LoginCodeGrpcServiceClient loginCodeClient) : IAuthenticationClient
{
    private readonly JwtTokenGrpcServiceClient _tokenClient = tokenClient;
    private readonly LoginCodeGrpcServiceClient _loginCodeClient = loginCodeClient;

    public async Task<string> GenerateJwtTokenAsync(string accountId)
    {
        var response = await _tokenClient.GenerateJwtTokenAsync(new GenerateJwtTokenRequest
        {
            AccountId = accountId
        });

        return response.Token;
    }

    public async Task<string?> ValidateJwtTokenAsync(string token)
    {
        var response = await _tokenClient.ValidateJwtTokenAsync(new ValidateJwtTokenRequest
        {
            Token = token
        });

        return response.AccountId;
    }

    public async Task<(int code, bool isCodeCreated, int ttl)> GenerateLoginCodeAsync(string email)
    {
        var response = await _loginCodeClient.GenerateLoginCodeAsync(new GenerateLoginCodeRequest
        {
            Email = email
        });

        return (response.Code, response.IsCodeCreated, response.Ttl);
    }

    public async Task<bool> ValidateLoginCodeAsync(string email, int code)
    {
        var response = await _loginCodeClient.ValidateLoginCodeAsync(new ValidateLoginCodeRequest
        {
            Email = email,
            Code = code
        });

        return response.IsValid;
    }
}
