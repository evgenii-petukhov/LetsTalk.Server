using Grpc.Core;
using Grpc.Core.Interceptors;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.FileStorage.Service.GrpcInterceptors;

public class JwtInterceptor(
    IAuthenticationClient authenticationClient,
    ISignPackageService signPackageService) : Interceptor
{
    private readonly IAuthenticationClient _authenticationClient = authenticationClient;
    private readonly ISignPackageService _signPackageService = signPackageService;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var token = context.RequestHeaders.Get("authorization");
        if (string.IsNullOrEmpty(token?.Value))
        {
            throw new UnauthorizedAccessException();
        }

        var accountId = await _authenticationClient.ValidateJwtTokenAsync(token.Value);
        if (string.IsNullOrEmpty(accountId))
        {
            throw new UnauthorizedAccessException();
        }

        context.UserState.Add("AccountId", accountId);
        var response = await continuation(request, context);

        _signPackageService.Sign(response);

        return response;
    }
}