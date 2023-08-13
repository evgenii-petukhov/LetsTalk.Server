using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.API.Models;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;
using LetsTalk.Server.Core.Features.Message.Queries.GetMessages;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.ImageProcessor.Models;
using LetsTalk.Server.LinkPreview.Models;
using LetsTalk.Server.Notifications.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class MessageController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly MessagingSettings _messagingSettings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProducer _messageNotificationProducer;
    private readonly IMessageProducer _linkPreviewRequestProducer;
    private readonly IMessageProducer _imageResizeRequestProducer;
    private readonly IMessageProducer _setImageDimensionsRequestProducer;

    public MessageController(
        IMediator mediator,
        IMapper mapper,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<MessagingSettings> messagingSettings)
    {
        _mediator = mediator;
        _mapper = mapper;
        _messagingSettings = messagingSettings.Value;
        _kafkaSettings = kafkaSettings.Value;
        _messageNotificationProducer = producerAccessor.GetProducer(_kafkaSettings.MessageNotification!.Producer);
        _linkPreviewRequestProducer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewRequest!.Producer);
        _imageResizeRequestProducer = producerAccessor.GetProducer(_kafkaSettings.ImageResizeRequest!.Producer);
        _setImageDimensionsRequestProducer = producerAccessor.GetProducer(_kafkaSettings.SetImageDimensionsRequest!.Producer);
    }

    [HttpGet("{recipientId}")]
    public async Task<ActionResult<List<MessageDto>>> GetAsync(int recipientId, [FromQuery(Name="page")] int pageIndex = 0, CancellationToken cancellationToken = default)
    {
        var senderId = GetAccountId();
        var query = new GetMessagesQuery(senderId, recipientId, pageIndex, _messagingSettings.MessagesPerPage);
        var messageDtos = await _mediator.Send(query, cancellationToken);
        await Task.WhenAll(messageDtos
            .Where(messageDto => messageDto.ImageId.HasValue && messageDto.ImagePreview == null)
            .Select(messageDto => _imageResizeRequestProducer.ProduceAsync(
                _kafkaSettings.ImageResizeRequest!.Topic,
                Guid.NewGuid().ToString(),
                new ImageResizeRequest
                {
                    SenderId = senderId,
                    RecipientId = recipientId,
                    MessageId = messageDto.Id,
                    ImageId = messageDto.ImageId!.Value
                })));
        await Task.WhenAll(messageDtos
            .Where(messageDto => messageDto.ImagePreview != null && (!messageDto.ImagePreview.Width.HasValue || !messageDto.ImagePreview.Height.HasValue))
            .Select(messageDto => _setImageDimensionsRequestProducer.ProduceAsync(
                _kafkaSettings.SetImageDimensionsRequest!.Topic,
                Guid.NewGuid().ToString(),
                new SetImageDimensionsRequest
                {
                    ImageId = messageDto.ImagePreview!.Id
                })));
        return Ok(messageDtos);
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> PostAsync(CreateMessageRequest request, CancellationToken cancellationToken)
    {
        var cmd = _mapper.Map<CreateMessageCommand>(request);
        var senderId = GetAccountId();
        cmd.SenderId = senderId;
        var response = await _mediator.Send(cmd, cancellationToken);
        await Task.WhenAll(
            _messageNotificationProducer.ProduceAsync(
                _kafkaSettings.MessageNotification!.Topic,
                Guid.NewGuid().ToString(),
                new Notification<MessageDto>[]
                {
                    new Notification<MessageDto> {
                        RecipientId = request.RecipientId,
                        Message = response.Dto! with
                        {
                            IsMine = false
                        }
                    },
                    new Notification<MessageDto> {
                        RecipientId = senderId,
                        Message = response.Dto with
                        {
                            IsMine = true
                        }
                    }
                }),
            string.IsNullOrWhiteSpace(response.Url) ? Task.CompletedTask : _linkPreviewRequestProducer.ProduceAsync(
                _kafkaSettings.LinkPreviewRequest!.Topic,
                Guid.NewGuid().ToString(),
                new LinkPreviewRequest
                {
                    SenderId = senderId,
                    RecipientId = request.RecipientId,
                    MessageId = response.Dto.Id,
                    Url = response.Url
                }),
            request.ImageId.HasValue ? _imageResizeRequestProducer.ProduceAsync(
                _kafkaSettings.ImageResizeRequest!.Topic,
                Guid.NewGuid().ToString(),
                new ImageResizeRequest
                {
                    SenderId = senderId,
                    RecipientId = request.RecipientId,
                    MessageId = response.Dto.Id,
                    ImageId = request.ImageId.Value
                }) : Task.CompletedTask);
        return Ok(response.Dto);
    }

    [HttpPut("MarkAsRead")]
    public async Task<ActionResult> MarkAsReadAsync(MarkAsReadRequest request, CancellationToken cancellationToken)
    {
        var cmd = _mapper.Map<ReadMessageCommand>(request);
        cmd.RecipientId = GetAccountId();
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }
}
