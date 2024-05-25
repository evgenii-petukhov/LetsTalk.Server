using AutoMapper;
using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.API.Core.Features.Chat.Commands.CreateIndividualChat;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class ChatProfile : Profile
{
	public ChatProfile()
	{
		CreateMap<CreateIndividualChatRequest, CreateIndividualChatCommand>();
		CreateMap<ChatServiceModel, ChatDto>();
    }
}
