using AutoMapper;
using KafkaFlow.Producers;
using LetsTalk.Server.API.Attributes;
using LetsTalk.Server.API.Models;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;
using LetsTalk.Server.Core.Features.Message.Queries.GetMessages;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
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
        private readonly IProducerAccessor _producerAccessor;
        private readonly KafkaSettings _kafkaSettings;

        public MessageController(
            IMediator mediator,
            IMapper mapper,
            IProducerAccessor producerAccessor,
            IOptions<KafkaSettings> kafkaSettings)
        {
            _mediator = mediator;
            _mapper = mapper;
            _producerAccessor = producerAccessor;
            _kafkaSettings = kafkaSettings.Value;
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
            cmd.SenderId = (int)HttpContext.Items["AccountId"]!;
            var response = await _mediator.Send(cmd);
            var producer = _producerAccessor.GetProducer(_kafkaSettings.MessageNotificationProducer);
            _ = producer.ProduceAsync(
                _kafkaSettings.MessageNotificationTopic,
                Guid.NewGuid().ToString(),
                new MessageNotification
                {
                    RecipientId = request.RecipientId,
                    Message = response.Dto
                });
            _ = producer.ProduceAsync(
                _kafkaSettings.LinkPreviewTopic,
                Guid.NewGuid().ToString(),
                new LinkPreviewRequest
                {
                    MessageId = response.Dto!.Id,
                    Url = response.Url
                });
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
    }
}
