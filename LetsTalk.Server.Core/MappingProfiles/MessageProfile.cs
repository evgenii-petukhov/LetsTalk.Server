using AutoMapper;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;
using LetsTalk.Server.Domain;
using LetsTalk.Server.API.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.API.Models.CreateMessage;

namespace LetsTalk.Server.Core.MappingProfiles;

public class MessageProfile : Profile
{
	public MessageProfile()
	{
		CreateMap<CreateMessageRequest, CreateMessageCommand>();
		CreateMap<CreateMessageCommand, Message>();
		CreateMap<Message, MessageDto>()
			.ForMember(x => x.Created, x => x.MapFrom(source => source.DateCreatedUnix));
		CreateMap<LinkPreview, LinkPreviewDto>();
        CreateMap<MarkAsReadRequest, ReadMessageCommand>();
    }
}
