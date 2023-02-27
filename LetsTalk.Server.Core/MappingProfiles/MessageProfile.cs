using AutoMapper;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Models.Message;

namespace LetsTalk.Server.Core.MappingProfiles;

public class MessageProfile : Profile
{
	public MessageProfile()
	{
		CreateMap<CreateMessageCommand, Message>();
		CreateMap<Message, MessageDto>();
	}
}
