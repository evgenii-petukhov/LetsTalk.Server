using Grpc.Core.Interceptors;
using Grpc.Core;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.FileStorageService.GrpcInterceptors;

public class JwtInterceptor : Interceptor
{
    private readonly IAuthenticationClient _authenticationClient;
    private readonly AuthenticationSettings _authenticationSettings;

    public JwtInterceptor(
        IAuthenticationClient authenticationClient,
        IOptions<AuthenticationSettings> options)
    {
        _authenticationClient = authenticationClient;
        _authenticationSettings = options.Value;
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

        var accountId = await _authenticationClient.ValidateJwtTokenAsync(_authenticationSettings.Url!, token.Value);
        if (!accountId.HasValue)
        {
            throw new UnauthorizedAccessException();
        }

        context.UserState.Add("AccountId", accountId);
        return await continuation(request, context);
    }
}