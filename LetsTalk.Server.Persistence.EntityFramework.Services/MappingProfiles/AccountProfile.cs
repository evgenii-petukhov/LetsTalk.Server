using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.MappingProfiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Domain.Account, AccountServiceModel>();
        CreateMap<Domain.ChatListItem, ContactServiceModel>();
    }
}
