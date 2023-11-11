using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices.MappingProfiles;

public class LinkPreviewProfile : Profile
{
    public LinkPreviewProfile()
    {
        CreateMap<Domain.LinkPreview, LinkPreviewAgnosticModel>();
    }
}
