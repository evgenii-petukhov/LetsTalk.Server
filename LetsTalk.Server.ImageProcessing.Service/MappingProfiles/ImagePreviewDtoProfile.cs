using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.ImageProcessing.Service.MappingProfiles;

public class ImagePreviewDtoProfile : Profile
{
    public ImagePreviewDtoProfile()
    {
        CreateMap<MessageServiceModel, ImagePreviewDto>()
            .ForMember(x => x.MessageId, x => x.MapFrom(m => m.Id))
            .ForMember(x => x.Id, x => x.MapFrom(m => m.ImagePreview!.Id));
    }
}
