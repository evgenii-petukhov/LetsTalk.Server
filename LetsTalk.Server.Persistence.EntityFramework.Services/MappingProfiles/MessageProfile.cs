using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.MappingProfiles;

public class MessageProfile: Profile
{
    public MessageProfile()
    {
        CreateMap<Domain.Message, MessageServiceModel>();
    }
}
