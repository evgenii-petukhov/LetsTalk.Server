namespace LetsTalk.Server.Core.Models.Authentication;

public class LoginResponse
{
    public string? Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Token { get; set; }
}
