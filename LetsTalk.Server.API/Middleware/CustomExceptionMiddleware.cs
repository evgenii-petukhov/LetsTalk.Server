using LetsTalk.Server.API.Middleware.Models;
using LetsTalk.Server.Core.Exceptions;
using System.Net;

namespace LetsTalk.Server.API.Middleware;

public class CustomExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ctx, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        CustomProblemDetails problem;
        HttpStatusCode statusCode;
        switch (ex)
        {
            case BadRequestException badRequestException:
                statusCode = HttpStatusCode.BadRequest;
                problem = new CustomProblemDetails
                {
                    Title = badRequestException.Message,
                    Status = (int)statusCode,
                    Type = nameof(BadRequestException),
                    Detail = badRequestException.InnerException?.Message,
                };
                break;
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                problem = new CustomProblemDetails
                {
                    Title = notFoundException.Message,
                    Status = (int)statusCode,
                    Type = nameof(NotFoundException),
                    Detail = notFoundException.InnerException?.Message
                };
                break;
            default:
                statusCode = HttpStatusCode.NotFound;
                problem = new CustomProblemDetails
                {
                    Title = ex.Message,
                    Status = (int)statusCode,
                    Type = nameof(HttpStatusCode.InternalServerError),
                    Detail = ex.StackTrace
                };
                break;
        }

        ctx.Response.StatusCode = (int)statusCode;
        await ctx.Response.WriteAsJsonAsync(problem);
    }
}
