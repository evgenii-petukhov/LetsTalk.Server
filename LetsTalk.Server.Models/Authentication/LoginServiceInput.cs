namespace LetsTalk.Server.Models.Authentication;

public class LoginServiceInput
{
    public string? Provider { get; set; }

    public string? Id { get; set; }

    public string? AuthToken { get; set; }
}
