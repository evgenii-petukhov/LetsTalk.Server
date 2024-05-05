using AutoMapper;
using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.Core.Features.Chat.Commands.CreateIndividualChatCommand;

namespace LetsTalk.Server.Core.MappingProfiles;

public class ChatProfile : Profile
{
	public ChatProfile()
	{
		CreateMap<CreateIndividualChatRequest, CreateIndividualChatCommand>();
    }
}
