using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Services.MappingProfiles;

public class AccountProfile: Profile
{
    public AccountProfile()
    {
        CreateMap<Contact, ContactServiceModel>();
        CreateMap<Account, AccountServiceModel>();
    }
}
