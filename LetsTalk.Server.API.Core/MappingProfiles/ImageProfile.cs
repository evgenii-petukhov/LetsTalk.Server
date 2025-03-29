using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class ImageProfile : Profile
{
	public ImageProfile()
	{
        CreateMap<ImageServiceModel, ImageDto>();
        CreateMap<ImageServiceModel, RemoveImageRequest>()
            .ForMember(x => x.ImageId, x => x.MapFrom(s => s.Id))
            .ForMember(x => x.FileStorageType, x => x.MapFrom(s => (FileStorageTypes)s.FileStorageTypeId));
        CreateMap<ImagePreviewServiceModel, ImagePreviewDto>();
    }
}
