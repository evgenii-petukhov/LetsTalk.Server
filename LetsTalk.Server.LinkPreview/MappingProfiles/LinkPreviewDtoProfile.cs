using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.LinkPreview.MappingProfiles;

public class LinkPreviewDtoProfile : Profile
{
    public LinkPreviewDtoProfile()
    {
        CreateMap<MessageServiceModel, LinkPreviewDto>()
            .ForMember(x => x.MessageId, x => x.MapFrom(m => m.Id))
            .ForMember(x => x.Title, x => x.MapFrom(m => m.LinkPreview!.Title))
            .ForMember(x => x.ImageUrl, x => x.MapFrom(m => m.LinkPreview!.ImageUrl))
            .ForMember(x => x.Url, x => x.MapFrom(m => m.LinkPreview!.Url));
    }
}
