using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.MappingProfiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Domain.Account, ProfileServiceModel>();
        CreateMap<Domain.Account, AccountServiceModel>();
    }
}
