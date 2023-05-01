namespace LetsTalk.Server.API.Models.Login;

public class LoginServiceInput
{
    public string? Provider { get; set; }

    public string? Id { get; set; }

    public string? AuthToken { get; set; }
}
