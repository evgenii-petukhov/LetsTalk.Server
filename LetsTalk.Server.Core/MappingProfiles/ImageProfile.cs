using AutoMapper;
using LetsTalk.Server.Caching.Abstractions.Models;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class ImageProfile : Profile
{
	public ImageProfile()
	{
        CreateMap<Image, ImagePreviewDto>();
        CreateMap<Image, ImagePreviewCacheEntry>();
        CreateMap<ImagePreviewCacheEntry, ImagePreviewDto>();
    }
}
