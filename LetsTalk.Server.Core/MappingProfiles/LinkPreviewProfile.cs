using AutoMapper;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class LinkPreviewProfile : Profile
{
	public LinkPreviewProfile()
	{
        CreateMap<Domain.LinkPreview, LinkPreviewDto>();
    }
}
