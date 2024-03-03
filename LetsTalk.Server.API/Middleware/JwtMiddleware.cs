using LetsTalk.Server.Authentication.Abstractions;
using System.Globalization;

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
            var accountId = await authenticationClient.ValidateJwtTokenAsync(token);
            if (accountId != null)
            {
                context.Items["AccountId"] = accountId;
            }
        }
        await _next(context);
    }
}