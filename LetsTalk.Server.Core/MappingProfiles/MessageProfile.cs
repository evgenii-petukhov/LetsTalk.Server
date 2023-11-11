using AutoMapper;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.DatabaseAgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Core.MappingProfiles;

public class MessageProfile : Profile
{
	public MessageProfile()
	{
		CreateMap<CreateMessageRequest, CreateMessageCommand>();
		CreateMap<Message, MessageDto>()
            .ForMember(x => x.Created, x => x.MapFrom(source => source.DateCreatedUnix));
        CreateMap<MessageAgnosticModel, MessageDto>()
            .ForMember(x => x.Created, x => x.MapFrom(source => source.DateCreatedUnix));
    }
}
