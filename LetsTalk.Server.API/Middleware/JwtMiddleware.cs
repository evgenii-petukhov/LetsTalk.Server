using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuthenticationClient authenticationClient,
        IOptions<AuthenticationSettings> options)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?
            .Split(" ")
            .Last();
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }

        var accountId = await authenticationClient.ValidateJwtTokenAsync(options.Value.Url!, token);
        if (!accountId.HasValue)
        {
            throw new UnauthorizedAccessException();
        }

        context.Items["AccountId"] = accountId;

        await _next(context);
    }
}