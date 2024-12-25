using LetsTalk.Server.API.Middleware.Models;
using LetsTalk.Server.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace LetsTalk.Server.API.Middleware;

public class CustomExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

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

    private static string GetExceptionText(Exception e)
    {
        return e.InnerException?.Message ?? e.Message;
    }

    private Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        CustomProblemDetails problem;
        int statusCode;
        switch (ex)
        {
            case BadRequestException badRequest:
                statusCode = (int)HttpStatusCode.BadRequest;
                problem = new CustomProblemDetails
                {
                    Title = badRequest.Message,
                    Status = statusCode,
                    Type = nameof(BadRequestException),
                    Detail = GetExceptionText(badRequest),
                    Errors = badRequest.ValidationErrors!
                };
                break;
            case NotFoundException notFound:
                statusCode = (int)HttpStatusCode.NotFound;
                problem = new CustomProblemDetails
                {
                    Title = notFound.Message,
                    Status = statusCode,
                    Type = nameof(NotFoundException),
                    Detail = GetExceptionText(notFound)
                };
                break;
            case UnauthorizedAccessException unauthorized:
                statusCode = (int)HttpStatusCode.Unauthorized;
                problem = new CustomProblemDetails
                {
                    Title = unauthorized.Message,
                    Status = statusCode,
                    Type = nameof(UnauthorizedAccessException),
                    Detail = GetExceptionText(unauthorized)
                };
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                problem = new CustomProblemDetails
                {
                    Title = ex.Message,
                    Status = statusCode,
                    Type = nameof(HttpStatusCode.InternalServerError),
                    Detail = GetExceptionText(ex),
                    StackTrace = ex.StackTrace
                };
                break;
        }

        ctx.Response.StatusCode = statusCode;
        ctx.Response.ContentType = "application/json";
        return ctx.Response.WriteAsJsonAsync(problem);
    }
}
