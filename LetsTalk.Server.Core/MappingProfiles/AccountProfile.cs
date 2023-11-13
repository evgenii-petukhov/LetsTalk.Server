using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<AccountServiceModel, AccountDto>();
        CreateMap<AccountListItem, AccountDto> ();
    }
}
