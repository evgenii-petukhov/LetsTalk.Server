using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Middleware.Models;

public class CustomProblemDetails : ProblemDetails
{
    public string? StackTrace { get; set; }

    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
