namespace LetsTalk.Server.API.Models.Login;

public class GenerateLoginCodeRequest
{
    public string? Email { get; set; }

    public long AntiSpamToken { get; set; }
}
