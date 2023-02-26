namespace LetsTalk.Server.Models.Account;

public class AccountDto
{
    public int Id { get; set; }

    public int AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}
