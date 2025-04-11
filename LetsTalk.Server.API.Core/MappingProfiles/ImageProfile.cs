using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class ImageProfile : Profile
{
	public ImageProfile()
	{
        CreateMap<ImageServiceModel, ImageDto>();
        CreateMap<ImageServiceModel, RemoveImageRequest>();
        CreateMap<ImagePreviewServiceModel, ImagePreviewDto>();
    }
}
