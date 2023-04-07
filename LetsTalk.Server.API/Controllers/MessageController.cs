using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.API.Attributes;
using LetsTalk.Server.API.Models;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;
using LetsTalk.Server.Core.Features.Message.Queries.GetMessages;
using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.LinkPreview.Models;
using LetsTalk.Server.Notifications.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IMessageProducer _messageNotificationProducer;
        private readonly IMessageProducer _linkPreviewRequestProducer;

        public MessageController(
            IMediator mediator,
            IMapper mapper,
            IProducerAccessor producerAccessor,
            IOptions<KafkaSettings> kafkaSettings)
        {
            _mediator = mediator;
            _mapper = mapper;
            _kafkaSettings = kafkaSettings.Value;
            _messageNotificationProducer = producerAccessor.GetProducer(_kafkaSettings.MessageNotification!.Producer);
            _linkPreviewRequestProducer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewRequest!.Producer);
        }

        [HttpGet]
        public async Task<ActionResult<List<MessageDto>>> Get(int recipientId)
        {
            var senderId = (int)HttpContext.Items["AccountId"]!;
            var query = new GetMessagesQuery(senderId, recipientId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> Post(CreateMessageRequest request)
        {
            var cmd = _mapper.Map<CreateMessageCommand>(request);
            var senderId = (int)HttpContext.Items["AccountId"]!;
            cmd.SenderId = senderId;
            var response = await _mediator.Send(cmd);
            await SendMessageNotification(request.RecipientId, response.Dto! with { IsMine = false });
            await SendMessageNotification(senderId, response.Dto! with { IsMine = true });
            if (!string.IsNullOrWhiteSpace(response.Url))
            {
                await _linkPreviewRequestProducer.ProduceAsync(
                    _kafkaSettings.LinkPreviewRequest!.Topic,
                    Guid.NewGuid().ToString(),
                    new LinkPreviewRequest
                    {
                        SenderId = senderId,
                        RecipientId = request.RecipientId,
                        MessageId = response.Dto!.Id,
                        Url = response.Url
                    });
            }
            return Ok(response.Dto);
        }

        [HttpPut("MarkAsRead")]
        public async Task<ActionResult> MarkAsRead(MarkAsReadRequest request)
        {
            var cmd = _mapper.Map<ReadMessageCommand>(request);
            cmd.RecipientId = (int)HttpContext.Items["AccountId"]!;
            await _mediator.Send(cmd);
            return Ok();
        }

        private async Task SendMessageNotification(int accountId, MessageDto messageDto)
        {
            await _messageNotificationProducer.ProduceAsync(
                _kafkaSettings.MessageNotification!.Topic,
                Guid.NewGuid().ToString(),
                new Notification<MessageDto>
                {
                    RecipientId = accountId,
                    Message = messageDto
                });
        }
    }
}
