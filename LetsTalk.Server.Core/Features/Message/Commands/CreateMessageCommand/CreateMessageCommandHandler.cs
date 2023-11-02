using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, CreateMessageResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IMessageProcessor _messageProcessor;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _memoryCache;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProducer _messageNotificationProducer;
    private readonly IMessageProducer _linkPreviewRequestProducer;
    private readonly IMessageProducer _imageResizeRequestProducer;

    public CreateMessageCommandHandler(
        IAccountRepository accountRepository,
        IMessageRepository messageRepository,
        IImageRepository imageRepository,
        IMessageProcessor messageProcessor,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings,
        IMemoryCache memoryCache)
    {
        _accountRepository = accountRepository;
        _messageRepository = messageRepository;
        _messageProcessor = messageProcessor;
        _imageRepository = imageRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _memoryCache = memoryCache;
        _kafkaSettings = kafkaSettings.Value;
        _messageNotificationProducer = producerAccessor.GetProducer(_kafkaSettings.MessageNotification!.Producer);
        _linkPreviewRequestProducer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewRequest!.Producer);
        _imageResizeRequestProducer = producerAccessor.GetProducer(_kafkaSettings.ImageResizeRequest!.Producer);
    }

    public async Task<CreateMessageResponse> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateMessageCommandValidator(_accountRepository, _imageRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var message = _mapper.Map<Domain.Message>(request);
        _messageProcessor.SetTextHtml(message, out string? url);
        await _messageRepository.CreateAsync(message, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        var messageDto = _mapper.Map<MessageDto>(message);

        await Task.WhenAll(
            _messageNotificationProducer.ProduceAsync(
                _kafkaSettings.MessageNotification!.Topic,
                Guid.NewGuid().ToString(),
                new Notification<MessageDto>[]
                {
                    new Notification<MessageDto> {
                        RecipientId = request.RecipientId!.Value,
                        Message = messageDto! with
                        {
                            IsMine = false
                        }
                    },
                    new Notification<MessageDto> {
                        RecipientId = request.SenderId!.Value,
                        Message = messageDto with
                        {
                            IsMine = true
                        }
                    }
                }),
            string.IsNullOrWhiteSpace(url) ? Task.CompletedTask : _linkPreviewRequestProducer.ProduceAsync(
                _kafkaSettings.LinkPreviewRequest!.Topic,
                Guid.NewGuid().ToString(),
                new LinkPreviewRequest
                {
                    MessageId = messageDto.Id,
                    Url = url
                }),
            request.ImageId.HasValue ? _imageResizeRequestProducer.ProduceAsync(
                _kafkaSettings.ImageResizeRequest!.Topic,
                Guid.NewGuid().ToString(),
                new ImageResizeRequest
                {
                    MessageId = messageDto.Id,
                    ImageId = request.ImageId.Value
                }) : Task.CompletedTask);

        _memoryCache.Remove($"messages_{request.SenderId}_{request.RecipientId}");

        return new CreateMessageResponse
        {
            Dto = messageDto,
            Url = url
        };
    }
}
