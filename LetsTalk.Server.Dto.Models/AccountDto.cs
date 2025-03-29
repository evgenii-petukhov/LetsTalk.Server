namespace LetsTalk.Server.Dto.Models;

public class AccountDto
{
    public string? Id { get; set; }

    public int AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public ImageDto? Image { get; set; }
}
