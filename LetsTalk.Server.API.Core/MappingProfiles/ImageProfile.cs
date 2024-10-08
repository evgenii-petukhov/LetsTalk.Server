﻿using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class ImageProfile : Profile
{
	public ImageProfile()
	{
        CreateMap<ImagePreviewServiceModel, ImagePreviewDto>();
    }
}
