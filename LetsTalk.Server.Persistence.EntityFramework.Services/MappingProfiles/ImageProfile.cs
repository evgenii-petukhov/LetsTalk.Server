using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.MappingProfiles;

public class ImageProfile : Profile
{
    public ImageProfile()
    {
        CreateMap<Domain.Image, ImagePreviewServiceModel>();
    }
}
