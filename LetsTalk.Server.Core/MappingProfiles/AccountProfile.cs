using AutoMapper;
using LetsTalk.Server.API.Models.UpdateProfile;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class AccountProfile: Profile
{
    public AccountProfile()
    {
        CreateMap<Account, AccountDto>();
        CreateMap<AccountWithUnreadCount, AccountDto>();
        CreateMap<UpdateProfileResponse, AccountDto>();
    }
}
