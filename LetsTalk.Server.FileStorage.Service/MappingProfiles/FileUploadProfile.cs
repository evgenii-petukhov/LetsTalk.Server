using AutoMapper;
using Google.Protobuf;
using LetsTalk.Server.FileStorage.Service.Protos;
using LetsTalk.Server.FileStorage.Service.Models;

namespace LetsTalk.Server.FileStorage.Service.MappingProfiles;

public class FileUploadProfile : Profile
{
    public FileUploadProfile()
    {
        CreateMap<FetchImageResponse, DownloadImageResponse>()
            .ForMember(x => x.Content, x => x.MapFrom(s => ByteString.CopyFrom(s.Content)));
    }
}
