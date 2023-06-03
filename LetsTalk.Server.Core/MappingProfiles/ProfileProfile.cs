using AutoMapper;
using LetsTalk.Server.API.Models.UpdateProfile;
using LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

namespace LetsTalk.Server.Core.MappingProfiles;

public class ProfileProfile: Profile
{
    public ProfileProfile()
    {
        CreateMap<UpdateProfileRequest, UpdateProfileCommand>();
    }
}
