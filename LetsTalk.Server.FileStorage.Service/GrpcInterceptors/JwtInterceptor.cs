using Grpc.Core;
using Grpc.Core.Interceptors;
using LetsTalk.Server.Authentication.Abstractions;

namespace LetsTalk.Server.FileStorage.Service.GrpcInterceptors;

public class JwtInterceptor : Interceptor
{
    private readonly IAuthenticationClient _authenticationClient;

    public JwtInterceptor(
        IAuthenticationClient authenticationClient)
    {
        _authenticationClient = authenticationClient;
    }

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
        return await continuation(request, context);
    }
}