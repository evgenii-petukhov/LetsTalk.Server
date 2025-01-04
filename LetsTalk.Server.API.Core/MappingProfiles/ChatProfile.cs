using AutoMapper;
using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.API.Core.Commands;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class ChatProfile : Profile
{
	public ChatProfile()
	{
		CreateMap<CreateIndividualChatRequest, CreateIndividualChatCommand>();
		CreateMap<ChatServiceModel, ChatDto>();
    }
}
