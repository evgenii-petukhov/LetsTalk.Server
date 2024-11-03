using AutoMapper;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.API.Core.Features.Message.Commands.SetLinkPreview;
using LetsTalk.Server.API.Core.Features.Message.Commands.SetExistingLinkPreview;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class MessageProfile : Profile
{
	public MessageProfile()
	{
		CreateMap<CreateMessageRequest, CreateMessageCommand>();
		CreateMap<Message, MessageDto>()
            .ForMember(x => x.Created, x => x.MapFrom(source => source.DateCreatedUnix));
        CreateMap<MessageServiceModel, MessageDto>()
            .ForMember(x => x.Created, x => x.MapFrom(source => source.DateCreatedUnix));
        CreateMap<SetLinkPreviewRequest, SetLinkPreviewCommand>();
        CreateMap<SetExistingLinkPreviewRequest, SetExistingLinkPreviewCommand>();
        CreateMap<SetImagePreviewRequest, SetImagePreviewCommand>();
        CreateMap<MessageServiceModel, LinkPreviewDto>()
            .ForMember(x => x.MessageId, x => x.MapFrom(m => m.Id))
            .ForMember(x => x.Title, x => x.MapFrom(m => m.LinkPreview!.Title))
            .ForMember(x => x.ImageUrl, x => x.MapFrom(m => m.LinkPreview!.ImageUrl))
            .ForMember(x => x.Url, x => x.MapFrom(m => m.LinkPreview!.Url));
        CreateMap<MessageServiceModel, ImagePreviewDto>()
            .ForMember(x => x.MessageId, x => x.MapFrom(m => m.Id))
            .ForMember(x => x.Id, x => x.MapFrom(m => m.ImagePreview!.Id));
    }
}
