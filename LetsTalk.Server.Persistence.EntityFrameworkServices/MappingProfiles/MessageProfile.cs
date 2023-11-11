using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices.MappingProfiles;

public class MessageProfile: Profile
{
    public MessageProfile()
    {
        CreateMap<Domain.Message, MessageAgnosticModel>();
    }
}
