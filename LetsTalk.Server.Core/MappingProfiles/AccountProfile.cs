using AutoMapper;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Account, AccountDto>();
        CreateMap<AccountListItem, AccountDto>();
    }
}
