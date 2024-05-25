using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Models.Login;

public class GenerateLoginCodeRequest
{
    public string? Email { get; set; }

    [FromQuery(Name = "_dt")]
    public long DateTimeUnix { get; set; }
}
