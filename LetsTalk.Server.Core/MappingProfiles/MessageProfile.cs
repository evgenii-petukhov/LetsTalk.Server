using AutoMapper;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Caching.Abstractions.Models;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class MessageProfile : Profile
{
	public MessageProfile()
	{
		CreateMap<CreateMessageRequest, CreateMessageCommand>();
		CreateMap<CreateMessageCommand, Message>();
		CreateMap<Message, MessageCacheEntry>()
            .ForMember(x => x.Created, x => x.MapFrom(source => source.DateCreatedUnix));
        CreateMap<Domain.LinkPreview, LinkPreviewDto>();
        CreateMap<Domain.LinkPreview, LinkPreviewCacheEntry>();
        CreateMap<MessageCacheEntry, MessageDto>();
        CreateMap<LinkPreviewCacheEntry, LinkPreviewDto>();
    }
}
