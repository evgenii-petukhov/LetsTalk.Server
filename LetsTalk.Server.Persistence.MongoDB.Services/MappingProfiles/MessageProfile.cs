using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Services.MappingProfiles;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageServiceModel>()
            .ForMember(x => x.FileStorageTypeId, x => x.MapFrom(source => source.Image != null ? source.Image.FileStorageTypeId : (int)FileStorageTypes.Local)); ;
    }
}
