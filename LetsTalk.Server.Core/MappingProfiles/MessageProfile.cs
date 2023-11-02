using AutoMapper;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Core.Features.Message.Queries.GetMessages;
using LetsTalk.Server.Core.Models.Caching;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class MessageProfile : Profile
{
	public MessageProfile()
	{
		CreateMap<CreateMessageRequest, CreateMessageCommand>();
		CreateMap<CreateMessageCommand, Message>();
		CreateMap<Message, MessageDto>()
			.ForMember(x => x.Created, x => x.MapFrom(source => source.DateCreatedUnix));
		CreateMap<Domain.LinkPreview, LinkPreviewDto>();
        CreateMap<Image, ImagePreviewDto>();
        CreateMap<GetMessagesQuery, MessageCacheKey>();
        CreateMap<CreateMessageCommand, MessageCacheKey>();
    }
}
