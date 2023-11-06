namespace LetsTalk.Server.Authentication.Models;

public class StoredToken
{
    public int AccountId { get; set; }

    public string? Token { get; set; }

    public DateTime ValidTo { get; set; }
}
