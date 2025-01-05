using Grpc.Core;
using LetsTalk.Server.Authentication.Abstractions;
using static LetsTalk.Server.Authentication.LoginCodeGrpcService;

namespace LetsTalk.Server.Authentication.Services;

public class LoginCodeGrpcService(ILoginCodeCacheService loginCodeCacheService) : LoginCodeGrpcServiceBase
{
    private readonly ILoginCodeCacheService _loginCodeCacheService = loginCodeCacheService;

    public override async Task<GenerateLoginCodeResponse> GenerateLoginCode(GenerateLoginCodeRequest request, ServerCallContext context)
    {
        var (code, isCodeCreated, ttl) = await _loginCodeCacheService.GenerateCodeAsync(request.Email);

        return new GenerateLoginCodeResponse
        {
            Code = code,
            IsCodeCreated = isCodeCreated,
            Ttl = (int)ttl.TotalSeconds
        };
    }

    public override async Task<ValidateLoginCodeResponse> ValidateLoginCode(ValidateLoginCodeRequest request, ServerCallContext context)
    {
        var isValid = await _loginCodeCacheService.ValidateCodeAsync(request.Email, request.Code);

        return new ValidateLoginCodeResponse
        {
            IsValid = isValid
        };
    }
}