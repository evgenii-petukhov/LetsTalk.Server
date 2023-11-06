using Grpc.Core;
using LetsTalk.Server.Authentication.Abstractions;
using static LetsTalk.Server.Authentication.JwtTokenGrpcService;

namespace LetsTalk.Server.Authentication.Services;

public class JwtTokenGrpcService : JwtTokenGrpcServiceBase
{
    private readonly IJwtCacheService _jwtCacheService;

    public JwtTokenGrpcService(IJwtCacheService jwtCacheService)
    {
        _jwtCacheService = jwtCacheService;
    }

    public override async Task<GenerateJwtTokenResponse> GenerateJwtToken(GenerateJwtTokenRequest request, ServerCallContext context)
    {
        var token = await _jwtCacheService.GenerateAsync(request.AccountId);

        return new GenerateJwtTokenResponse
        {
            Token = token
        };
    }

    public override async Task<ValidateJwtTokenResponse> ValidateJwtToken(ValidateJwtTokenRequest request, ServerCallContext context)
    {
        var accountId = await _jwtCacheService.GetAccountIdAsync(request.Token);

        return new ValidateJwtTokenResponse
        {
            AccountId = accountId
        };
    }
}