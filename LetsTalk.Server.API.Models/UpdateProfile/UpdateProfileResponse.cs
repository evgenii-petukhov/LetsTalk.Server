namespace LetsTalk.Server.API.Models.UpdateProfile;

public class UpdateProfileResponse
{
    public string? Email { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? ImageId { get; set; }
}
