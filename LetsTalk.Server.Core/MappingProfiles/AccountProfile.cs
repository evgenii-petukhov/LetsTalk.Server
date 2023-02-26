using AutoMapper;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Models.Account;

namespace LetsTalk.Server.Core.MappingProfiles
{
    public class AccountProfile: Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountDto>();
        }
    }
}
