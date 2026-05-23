using AutoMapper;
using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.Models.Dtos;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class ChatProfile : Profile
{
	public ChatProfile()
	{
		CreateMap<CreateIndividualChatRequest, CreateIndividualChatCommand>();
		CreateMap<ChatServiceModel, ChatDto>();
    }
}
