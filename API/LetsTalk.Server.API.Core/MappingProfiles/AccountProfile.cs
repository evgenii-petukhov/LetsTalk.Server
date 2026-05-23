using AutoMapper;
using LetsTalk.Server.Models.Dtos;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<ChatServiceModel, ChatDto>();
        CreateMap<AccountServiceModel, AccountDto>();
    }
}
