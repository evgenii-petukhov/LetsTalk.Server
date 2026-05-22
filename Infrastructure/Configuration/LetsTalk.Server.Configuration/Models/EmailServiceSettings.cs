namespace LetsTalk.Server.Configuration.Models;

public class EmailServiceSettings
{
    public string? SenderEmail { get; set; }

    public string? SenderName { get; set; }

    public string? Server { get; set; }

    public int Port { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }
}
