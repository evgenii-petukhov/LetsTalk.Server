using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.MappingProfiles;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Domain.Chat, ChatServiceModel>();
    }
}
