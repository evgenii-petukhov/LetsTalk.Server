using AutoMapper;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MassTransit;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.SetLinkPreview;

public class SetImagePreviewCommandHandler(
    IMessageAgnosticService messageAgnosticService,
    IChatAgnosticService chatAgnosticService,
    ITopicProducer<string, Notification<ImagePreviewDto>> producer,
    IMapper mapper,
    IMessageCacheManager messageCacheManager) : IRequestHandler<SetImagePreviewCommand, Unit>
{
    private readonly IMessageAgnosticService _messageAgnosticService = messageAgnosticService;
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly IMapper _mapper = mapper;
    private readonly ITopicProducer<string, Notification<ImagePreviewDto>> _producer = producer;
    private readonly IMessageCacheManager _messageCacheManager = messageCacheManager;

    public async Task<Unit> Handle(SetImagePreviewCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageAgnosticService.SaveImagePreviewAsync(
            request.MessageId!,
            request.Filename!,
            request.ImageFormat,
            request.Width,
            request.Height,
            cancellationToken: cancellationToken);

        var accountIds = await _chatAgnosticService.GetChatMemberAccountIdsAsync(request.ChatId!, cancellationToken);
        var imagePreviewDto = _mapper.Map<ImagePreviewDto>(message);

        await Task.WhenAll([
            Task.WhenAll(accountIds.Select(accountId => _producer.Produce(
                Guid.NewGuid().ToString(),
                new Notification<ImagePreviewDto>
                {
                    RecipientId = accountId,
                    Message = imagePreviewDto
                }, cancellationToken))),
            _messageCacheManager.ClearAsync(message.ChatId!)
        ]);

        return Unit.Value;
    }
}
