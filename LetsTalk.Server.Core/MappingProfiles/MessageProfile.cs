using AutoMapper;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;
using LetsTalk.Server.Core.Helpers;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Models.Message;

namespace LetsTalk.Server.Core.MappingProfiles;

public class MessageProfile : Profile
{
	public MessageProfile()
	{
		CreateMap<CreateMessageRequest, CreateMessageCommand>();
		CreateMap<CreateMessageCommand, Message>();
		CreateMap<Message, MessageDto>()
			.ForMember(x => x.AccountId, x => x.MapFrom(source => source.SenderId))
			.ForMember(x => x.CreatedUnixTime, x => x.MapFrom(source => DateTimeHelper.ConvertToUnixTimestamp(source.DateCreated)));
        CreateMap<MarkAsReadRequest, ReadMessageCommand>();
    }
}
