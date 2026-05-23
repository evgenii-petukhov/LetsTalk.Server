using Grpc.Core;
using LetsTalk.Server.Authentication.Abstractions;
using Microsoft.AspNetCore.Http;

namespace LetsTalk.Server.API.Middleware;

public class JwtMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        IAuthenticationClient authenticationClient)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?
            .Split(" ")
            .Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                context.Items["AccountId"] = await authenticationClient.ValidateJwtTokenAsync(token!)
                    ?? throw new UnauthorizedAccessException("Invalid token");
            }
            catch(RpcException)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
        }

        await _next(context);
    }
}