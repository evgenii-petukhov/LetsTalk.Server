using LetsTalk.Server.API.Models.Messages;

namespace LetsTalk.Server.API.Models.UpdateProfile;

public class UpdateProfileRequest
{
    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public ImageRequestModel? Image { get; set; }
}
