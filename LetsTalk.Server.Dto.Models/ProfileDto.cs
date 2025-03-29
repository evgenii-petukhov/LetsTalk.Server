namespace LetsTalk.Server.Dto.Models;

public class ProfileDto
{
    public string? Id { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public ImageDto? Image { get; set; }
}
