namespace LetsTalk.Server.Authentication.Models;

public class StoredToken
{
    public string? AccountId { get; set; }

    public string? Token { get; set; }

    public DateTime ValidTo { get; set; }
}
