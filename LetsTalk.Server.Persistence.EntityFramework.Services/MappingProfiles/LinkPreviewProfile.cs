using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.MappingProfiles;

public class LinkPreviewProfile : Profile
{
    public LinkPreviewProfile()
    {
        CreateMap<Domain.LinkPreview, LinkPreviewServiceModel>();
    }
}
