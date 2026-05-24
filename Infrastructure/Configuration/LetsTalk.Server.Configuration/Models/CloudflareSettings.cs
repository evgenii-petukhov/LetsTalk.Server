namespace LetsTalk.Server.Configuration.Models;

public class CloudflareSettings
{
    public string? TurnTokenId { get; set; }

    public string? ApiToken { get; set; }

    public int TokenTtl { get; set; }
}
