﻿using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Services.MappingProfiles;

public class UploadProfile : Profile
{
    public UploadProfile()
    {
        CreateMap<Image, ImageServiceModel>();
        CreateMap<Image, ImagePreviewServiceModel>();
        CreateMap<Models.File, FileServiceModel>();
    }
}