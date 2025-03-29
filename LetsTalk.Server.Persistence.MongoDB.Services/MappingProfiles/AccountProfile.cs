using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Services.MappingProfiles;

public class AccountProfile: Profile
{
    public AccountProfile()
    {
        CreateMap<Image, ImageServiceModel>();
        CreateMap<Account, ProfileServiceModel>();
        CreateMap<Account, AccountServiceModel>();
    }
}
