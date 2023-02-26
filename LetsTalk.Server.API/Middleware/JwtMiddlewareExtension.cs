namespace LetsTalk.Server.API.Middleware;

public static class JwtMiddlewareExtension
{
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<JwtMiddleware>();
    }
}