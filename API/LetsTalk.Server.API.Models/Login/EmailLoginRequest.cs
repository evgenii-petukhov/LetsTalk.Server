namespace LetsTalk.Server.API.Models.Login;

public class EmailLoginRequest
{
    public string? Email { get; set; }

    public int Code { get; set; }

    public long AntiSpamToken { get; set; }
}