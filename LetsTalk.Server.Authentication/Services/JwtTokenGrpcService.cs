using Grpc.Core;
using LetsTalk.Server.Authentication.Abstractions;
using static LetsTalk.Server.Authentication.JwtTokenGrpcService;

namespace LetsTalk.Server.Authentication.Services;

public class JwtTokenGrpcService : JwtTokenGrpcServiceBase
{
    private readonly IJwtStorageService _jwtStorageService;

    public JwtTokenGrpcService(IJwtStorageService jwtStorageService)
    {
        _jwtStorageService = jwtStorageService;
    }

    public override async Task<GenerateJwtTokenResponse> GenerateJwtToken(GenerateJwtTokenRequest request, ServerCallContext context)
    {
        var token = await _jwtStorageService.GenerateJwtToken(request.AccountId);

        return new GenerateJwtTokenResponse
        {
            Token = token.Token
        };
    }

    public override async Task<ValidateJwtTokenResponse> ValidateJwtToken(ValidateJwtTokenRequest request, ServerCallContext context)
    {
        var storedToken = await _jwtStorageService.GetStoredTokenAsync(request.Token);

        return new ValidateJwtTokenResponse
        {
            AccountId = storedToken!.AccountId
        };
    }
}