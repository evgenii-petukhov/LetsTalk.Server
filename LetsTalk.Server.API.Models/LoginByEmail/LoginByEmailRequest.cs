namespace LetsTalk.Server.API.Models.LoginByEmail;

public class LoginByEmailRequest
{
    public string? Email { get; set; }

    public int Code { get; set; }
}