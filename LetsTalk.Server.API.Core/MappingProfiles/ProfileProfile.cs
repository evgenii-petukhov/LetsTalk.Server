using AutoMapper;
using LetsTalk.Server.API.Models.Profile;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.API.Core.Commands;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class ProfileProfile : Profile
{
    public ProfileProfile()
    {
        CreateMap<ProfileServiceModel, ProfileDto>();
        CreateMap<UpdateProfileRequest, UpdateProfileCommand>();
        CreateMap<Account, ProfileDto>();
    }
}
