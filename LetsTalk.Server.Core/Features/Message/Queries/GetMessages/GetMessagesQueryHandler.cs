using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageProcessor _messageProcessor;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProducer _imageResizeRequestProducer;
    private readonly IMessageProducer _setImageDimensionsRequestProducer;

    public GetMessagesQueryHandler(
        IMessageRepository messageRepository,
        IMessageProcessor messageProcessor,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings)
    {
        _messageRepository = messageRepository;
        _messageProcessor = messageProcessor;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _kafkaSettings = kafkaSettings.Value;
        _imageResizeRequestProducer = producerAccessor.GetProducer(_kafkaSettings.ImageResizeRequest!.Producer);
        _setImageDimensionsRequestProducer = producerAccessor.GetProducer(_kafkaSettings.SetImageDimensionsRequest!.Producer);
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetPagedAsTrackingAsync(request.SenderId, request.RecipientId, request.PageIndex, request.MessagesPerPage, cancellationToken);

        messages
            .Where(message => !message.IsRead)
            .ToList()
            .ForEach(message => message.MarkAsRead());

        var messagesToProcess = messages
            .Where(message => message.TextHtml == null && !message.ImageId.HasValue)
            .ToList();

        Parallel.ForEach(messagesToProcess, message => _messageProcessor.SetTextHtml(message, out _));

        await _unitOfWork.SaveAsync(cancellationToken);

        var messageDtos = _mapper.Map<List<MessageDto>>(messages)
            .ConvertAll(messageDto =>
            {
                messageDto.IsMine = messageDto.SenderId == request.SenderId;
                return messageDto;
            });

        var imageResizeTasks = messageDtos
            .Where(messageDto => messageDto.ImageId.HasValue && messageDto.ImagePreview == null)
            .Select(messageDto => _imageResizeRequestProducer.ProduceAsync(
                _kafkaSettings.ImageResizeRequest!.Topic,
                Guid.NewGuid().ToString(),
                new ImageResizeRequest
                {
                    SenderId = request.SenderId,
                    RecipientId = request.RecipientId,
                    MessageId = messageDto.Id,
                    ImageId = messageDto.ImageId!.Value
                }));
        var setImageDimensionsTasks = messageDtos
            .Where(messageDto => messageDto.ImageId.HasValue && messageDto.ImagePreview != null && (!messageDto.ImagePreview.Width.HasValue || !messageDto.ImagePreview.Height.HasValue))
            .Select(messageDto => _setImageDimensionsRequestProducer.ProduceAsync(
                _kafkaSettings.SetImageDimensionsRequest!.Topic,
                Guid.NewGuid().ToString(),
                new SetImageDimensionsRequest
                {
                    ImageId = messageDto.ImagePreview!.Id
                }));

        await Task.WhenAll(imageResizeTasks.Concat(setImageDimensionsTasks));

        return messageDtos;
    }
}
