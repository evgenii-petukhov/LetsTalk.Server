using AutoMapper;
using LetsTalk.Server.Models.Dtos;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class LinkPreviewProfile : Profile
{
	public LinkPreviewProfile()
	{
        CreateMap<LinkPreviewServiceModel, LinkPreviewDto>();
    }
}
