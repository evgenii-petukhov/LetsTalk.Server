using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.MappingProfiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Domain.Account, ProfileServiceModel>()
            .ForMember(x => x.FileStorageTypeId, x => x.MapFrom(source => source.Image != null ? source.Image.FileStorageTypeId : (int)FileStorageTypes.Local));
        CreateMap<Domain.Account, AccountServiceModel>()
            .ForMember(x => x.FileStorageTypeId, x => x.MapFrom(source => source.Image != null ? source.Image.FileStorageTypeId : (int)FileStorageTypes.Local));
    }
}
