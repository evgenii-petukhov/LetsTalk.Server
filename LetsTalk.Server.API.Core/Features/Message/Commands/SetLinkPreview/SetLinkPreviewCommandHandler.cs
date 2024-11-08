using AutoMapper;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.SetLinkPreview;

public class SetLinkPreviewCommandHandler(
    IMessageAgnosticService messageAgnosticService,
    ILinkPreviewAgnosticService linkPreviewAgnosticService,
    IChatAgnosticService chatAgnosticService,
    IProducer<Notification<LinkPreviewDto>> producer,
    IMapper mapper,
    IMessageCacheManager messageCacheManager) : IRequestHandler<SetLinkPreviewCommand, Unit>
{
    private readonly IMessageAgnosticService _messageAgnosticService = messageAgnosticService;
    private readonly ILinkPreviewAgnosticService _linkPreviewAgnosticService = linkPreviewAgnosticService;
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly IMapper _mapper = mapper;
    private readonly IProducer<Notification<LinkPreviewDto>> _producer = producer;
    private readonly IMessageCacheManager _messageCacheManager = messageCacheManager;

    public async Task<Unit> Handle(SetLinkPreviewCommand request, CancellationToken cancellationToken)
    {
        MessageServiceModel message;
        try
        {
            message = await _messageAgnosticService.SetLinkPreviewAsync(
                request.MessageId!,
                request.Url!,
                request.Title!,
                request.ImageUrl!,
                cancellationToken);
        }
        catch
        {
            var linkPreviewId = await _linkPreviewAgnosticService.GetIdByUrlAsync(request.Url!, cancellationToken);

            message = await _messageAgnosticService.SetLinkPreviewAsync(
                request.MessageId!,
                linkPreviewId!,
                cancellationToken);
        }

        var accountIds = await _chatAgnosticService.GetChatMemberAccountIdsAsync(request.ChatId!, cancellationToken);
        var linkPreviewDto = _mapper.Map<LinkPreviewDto>(message);

        await Task.WhenAll([
            Task.WhenAll(accountIds.Select(accountId => _producer.PublishAsync(
                new Notification<LinkPreviewDto>
                {
                    RecipientId = accountId,
                    Message = linkPreviewDto
                }, cancellationToken))),
            _messageCacheManager.ClearAsync(message.ChatId!)
        ]);

        return Unit.Value;
    }
}
