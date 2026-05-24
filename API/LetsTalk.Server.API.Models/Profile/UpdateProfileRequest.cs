using LetsTalk.Server.API.Models.Message;

namespace LetsTalk.Server.API.Models.Profile;

public class UpdateProfileRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public ImageRequestModel? Image { get; set; }
}
