using Grpc.Core;
using LetsTalk.Server.Abstractions.Authentication;

namespace LetsTalk.Server.Authentication.Services;

public class JwtTokenService : JwtTokenGrpcService.JwtTokenGrpcServiceBase
{
    private readonly IJwtService _jwtService;

    public JwtTokenService(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public override Task<GenerateJwtTokenResponse> GenerateJwtToken(GenerateJwtTokenRequest request, ServerCallContext context)
    {
        var token = _jwtService.GenerateJwtToken(request.AccountId);

        return Task.FromResult(new GenerateJwtTokenResponse
        {
            Token = token
        });
    }

    public override Task<ValidateJwtTokenResponse> ValidateJwtToken(ValidateJwtTokenRequest request, ServerCallContext context)
    {
        var accountId = _jwtService.ValidateJwtToken(request.Token);

        return Task.FromResult(new ValidateJwtTokenResponse
        {
            AccountId = accountId
        });
    }
}