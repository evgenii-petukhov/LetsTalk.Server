using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Services.MappingProfiles;

public class AccountProfile: Profile
{
    public AccountProfile()
    {
        CreateMap<Account, ProfileServiceModel>()
            .ForMember(x => x.FileStorageTypeId, x => x.MapFrom(source => source.Image != null ? source.Image.FileStorageTypeId : (int)FileStorageTypes.Local)); ;
        CreateMap<Account, AccountServiceModel>()
            .ForMember(x => x.FileStorageTypeId, x => x.MapFrom(source => source.Image != null ? source.Image.FileStorageTypeId : (int)FileStorageTypes.Local)); ;
    }
}
