using Microsoft.AspNetCore.Builder;

namespace LetsTalk.Server.API.Middleware;

public static class CustomExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CustomExceptionMiddleware>();
    }
}