using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices.MappingProfiles;

public class ImageProfile : Profile
{
    public ImageProfile()
    {
        CreateMap<Domain.Image, ImagePreviewServiceModel>();
        CreateMap<Domain.Image, ImageServiceModel>();
        CreateMap<Domain.File, FileServiceModel>();
    }
}
