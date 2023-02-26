using LetsTalk.Server.Abstractions.Authentication;

namespace LetsTalk.Server.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IJwtService jwtService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var accountId = jwtService.ValidateJwtToken(token);
        if (accountId != null)
        {
            context.Items["AccountId"] = accountId.Value;
        }

        await _next(context);
    }
}