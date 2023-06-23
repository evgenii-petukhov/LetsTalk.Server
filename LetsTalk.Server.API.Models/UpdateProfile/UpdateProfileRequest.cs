namespace LetsTalk.Server.API.Models.UpdateProfile;

public class UpdateProfileRequest
{
    public string? Email { get; set; }

    public int? ImageId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}
