using Grpc.Core;
using LetsTalk.Server.Authentication.Abstractions;
using static LetsTalk.Server.Authentication.JwtTokenGrpcService;

namespace LetsTalk.Server.Authentication.Services;

public class JwtTokenGrpcService : JwtTokenGrpcServiceBase
{
    private readonly IJwtService _jwtService;

    public JwtTokenGrpcService(IJwtService jwtService)
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

    public override async Task<ValidateJwtTokenResponse> ValidateJwtToken(ValidateJwtTokenRequest request, ServerCallContext context)
    {
        var accountId = await _jwtService.ValidateJwtTokenAsync(request.Token);

        return new ValidateJwtTokenResponse
        {
            AccountId = accountId
        };
    }
}