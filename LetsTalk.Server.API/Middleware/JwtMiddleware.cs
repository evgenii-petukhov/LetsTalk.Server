using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Models.Authentication;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context, 
        IAuthenticationClient authenticationClient,
        IOptions<AuthenticationSettings> options)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?
            .Split(" ")
            .Last();

        var accountId = await authenticationClient.ValidateJwtToken(options.Value.Url, token);
        if (accountId != null)
        {
            context.Items["AccountId"] = accountId;
        }

        await _next(context);
    }
}