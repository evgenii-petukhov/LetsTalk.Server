using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.MappingProfiles;

public class MessageProfile: Profile
{
    public MessageProfile()
    {
        CreateMap<Domain.Message, MessageServiceModel>()
            .ForMember(x => x.FileStorageTypeId, x => x.MapFrom(source => source.Image != null ? source.Image.FileStorageTypeId : (int)FileStorageTypes.Local));
    }
}
