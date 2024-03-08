using LetsTalk.Server.Authentication.Abstractions;

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
            var accountId = await authenticationClient.ValidateJwtTokenAsync(token!);
            if (accountId == null)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
            else
            {
                context.Items["AccountId"] = accountId;
            }
        }

        await _next(context);
    }
}