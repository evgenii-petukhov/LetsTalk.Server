using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.API.Models.UpdateProfile;

public class UpdateProfileResponse : AccountDto
{
    public int? ImageId { get; set; }
}
