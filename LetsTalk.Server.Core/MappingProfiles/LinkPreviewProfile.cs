using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class LinkPreviewProfile : Profile
{
	public LinkPreviewProfile()
	{
        CreateMap<LinkPreviewServiceModel, LinkPreviewDto>();
    }
}
