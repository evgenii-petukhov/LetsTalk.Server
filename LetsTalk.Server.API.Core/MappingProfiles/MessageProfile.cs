﻿using AutoMapper;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class MessageProfile : Profile
{
	public MessageProfile()
	{
		CreateMap<CreateMessageRequest, CreateMessageCommand>();
		CreateMap<Message, MessageDto>()
            .ForMember(x => x.Created, x => x.MapFrom(source => source.DateCreatedUnix));
        CreateMap<MessageServiceModel, MessageDto>()
            .ForMember(x => x.Created, x => x.MapFrom(source => source.DateCreatedUnix));
    }
}
