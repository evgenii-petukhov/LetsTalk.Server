using AutoMapper;
using LetsTalk.Server.API.Models.UpdateProfile;
using LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class ProfileProfile : Profile
{
    public ProfileProfile()
    {
        CreateMap<UpdateProfileRequest, UpdateProfileCommand>();
        CreateMap<Account, ProfileDto>();
    }
}
