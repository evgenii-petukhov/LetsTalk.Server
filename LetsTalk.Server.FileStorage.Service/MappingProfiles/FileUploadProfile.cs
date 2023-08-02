using AutoMapper;
using Google.Protobuf;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.FileStorage.Service.Protos;

namespace LetsTalk.Server.FileStorage.Service.MappingProfiles;

public class FileUploadProfile : Profile
{
    public FileUploadProfile()
    {
        CreateMap<ImageDto, DownloadImageResponse>()
            .ForMember(x => x.Content, x => x.MapFrom(s => ByteString.CopyFrom(s.Content)));
    }
}
