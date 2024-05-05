using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Services.MappingProfiles;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Chat, ChatServiceModel>();
    }
}
