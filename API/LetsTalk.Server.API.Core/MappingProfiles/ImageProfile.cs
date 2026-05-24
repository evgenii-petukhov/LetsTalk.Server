using AutoMapper;
using LetsTalk.Server.Models.Dtos;
using LetsTalk.Server.Models.Kafka;
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
