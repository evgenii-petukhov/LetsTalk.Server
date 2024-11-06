using AutoMapper;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using MediatR;
using MassTransit;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;

public class CreateMessageCommandHandler(
    IChatAgnosticService chatAgnosticService,
    IHtmlGenerator htmlGenerator,
    IMapper mapper,
    IMessageCacheManager messageCacheManager,
    IMessageAgnosticService messageAgnosticService,
    ILinkPreviewAgnosticService linkPreviewAgnosticService,
    ITopicProducer<string, Notification<MessageDto>> notificationProducer,
    ITopicProducer<string, LinkPreviewRequest> linkPreviewProducer,
    ITopicProducer<string, ImageResizeRequest> imageResizeProducer
) : IRequestHandler<CreateMessageCommand, CreateMessageResponse>
{
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly IHtmlGenerator _htmlGenerator = htmlGenerator;
    private readonly IMapper _mapper = mapper;
    private readonly IMessageCacheManager _messageCacheManager = messageCacheManager;
    private readonly IMessageAgnosticService _messageAgnosticService = messageAgnosticService;
    private readonly ILinkPreviewAgnosticService _linkPreviewAgnosticService = linkPreviewAgnosticService;
    private readonly ITopicProducer<string, Notification<MessageDto>> _notificationProducer = notificationProducer;
    private readonly ITopicProducer<string, LinkPreviewRequest> _linkPreviewProducer = linkPreviewProducer;
    private readonly ITopicProducer<string, ImageResizeRequest> _imageResizeProducer = imageResizeProducer;

    public async Task<CreateMessageResponse> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateMessageCommandValidator(_chatAgnosticService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var (html, url) = _htmlGenerator.GetHtml(request.Text!);

        var linkPreviewId = string.IsNullOrWhiteSpace(url)
            ? null
            : await _linkPreviewAgnosticService.GetIdByUrlAsync(url, cancellationToken);

        var message = request.Image == null
            ? await _messageAgnosticService.CreateMessageAsync(
                request.SenderId!,
                request.ChatId!,
                request.Text!,
                html!,
                linkPreviewId!,
                cancellationToken)
            : await _messageAgnosticService.CreateMessageAsync(
                request.SenderId!,
                request.ChatId!,
                request.Text!,
                html!,
                request.Image.Id!,
                request.Image.Width,
                request.Image.Height,
                (ImageFormats)request.Image.ImageFormat,
                cancellationToken);

        await _messageCacheManager.ClearAsync(request.ChatId!);

        var messageDto = _mapper.Map<MessageDto>(message);

        var accountIds = await _chatAgnosticService.GetChatMemberAccountIdsAsync(request.ChatId!, cancellationToken);

        await Task.WhenAll(
            Task.WhenAll(accountIds.Select(accountId => _notificationProducer.Produce(
                Guid.NewGuid().ToString(),
                new Notification<MessageDto>
                {
                    RecipientId = accountId,
                    Message = messageDto with
                    {
                        IsMine = accountId == request.SenderId
                    }
                }, cancellationToken))),
            (string.IsNullOrWhiteSpace(url) || !string.IsNullOrWhiteSpace(linkPreviewId)) ? Task.CompletedTask : _linkPreviewProducer.Produce(
                Guid.NewGuid().ToString(),
                new LinkPreviewRequest
                {
                    AccountIds = accountIds,
                    MessageId = messageDto.Id,
                    Url = url,
                    ChatId = request.ChatId
                }, cancellationToken),
            request.Image == null ? Task.CompletedTask : _imageResizeProducer.Produce(
                Guid.NewGuid().ToString(),
                new ImageResizeRequest
                {
                    AccountIds = accountIds,
                    MessageId = messageDto.Id,
                    ImageId = request.Image.Id,
                    ChatId = request.ChatId
                }, cancellationToken));

        return new CreateMessageResponse
        {
            Dto = messageDto,
            Url = url
        };
    }
}
