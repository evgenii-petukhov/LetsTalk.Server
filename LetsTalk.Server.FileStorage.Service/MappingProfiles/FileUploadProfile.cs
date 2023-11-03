using AutoMapper;
using Google.Protobuf;
using LetsTalk.Server.Caching.Abstractions.Models;
using LetsTalk.Server.FileStorage.Service.Protos;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;

namespace LetsTalk.Server.FileStorage.Service.MappingProfiles;

public class FileUploadProfile : Profile
{
    public FileUploadProfile()
    {
        CreateMap<ImageCacheEntry, DownloadImageResponse>()
            .ForMember(x => x.Content, x => x.MapFrom(s => ByteString.CopyFrom(s.Content)));
        CreateMap<FetchImageResponse, ImageCacheEntry>();
    }
}
