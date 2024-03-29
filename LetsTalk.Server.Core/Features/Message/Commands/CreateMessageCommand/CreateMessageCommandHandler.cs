﻿using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.SignPackage.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, CreateMessageResponse>
{
    private readonly IAccountAgnosticService _accountAgnosticService;
    private readonly IHtmlGenerator _htmlGenerator;
    private readonly IMapper _mapper;
    private readonly IMessageCacheManager _messageCacheManager;
    private readonly IMessageAgnosticService _messageAgnosticService;
    private readonly ISignPackageService _signPackageService;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProducer _messageNotificationProducer;
    private readonly IMessageProducer _linkPreviewRequestProducer;
    private readonly IMessageProducer _imageResizeRequestProducer;

    public CreateMessageCommandHandler(
        IAccountAgnosticService accountAgnosticService,
        IHtmlGenerator htmlGenerator,
        IMapper mapper,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings,
        IMessageCacheManager messageCacheManager,
        IMessageAgnosticService messageDatabaseAgnosticService,
        ISignPackageService signPackageService)
    {
        _accountAgnosticService = accountAgnosticService;
        _htmlGenerator = htmlGenerator;
        _mapper = mapper;
        _messageCacheManager = messageCacheManager;
        _messageAgnosticService = messageDatabaseAgnosticService;
        _signPackageService = signPackageService;
        _kafkaSettings = kafkaSettings.Value;
        _messageNotificationProducer = producerAccessor.GetProducer(_kafkaSettings.MessageNotification!.Producer);
        _linkPreviewRequestProducer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewRequest!.Producer);
        _imageResizeRequestProducer = producerAccessor.GetProducer(_kafkaSettings.ImageResizeRequest!.Producer);
    }

    public async Task<CreateMessageResponse> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateMessageCommandValidator(_accountAgnosticService, _signPackageService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var (html, url) = _htmlGenerator.GetHtml(request.Text!);

        var message = request.Image == null
            ? await _messageAgnosticService.CreateMessageAsync(
                request.SenderId!,
                request.RecipientId!,
                request.Text!,
                html!,
                cancellationToken)
            : await _messageAgnosticService.CreateMessageAsync(
                request.SenderId!,
                request.RecipientId!,
                request.Text!,
                html!,
                request.Image.Id!,
                request.Image.Width,
                request.Image.Height,
                (ImageFormats)request.Image.ImageFormat,
                cancellationToken);

        var messageDto = _mapper.Map<MessageDto>(message);

        await Task.WhenAll(
            _messageNotificationProducer.ProduceAsync(
                _kafkaSettings.MessageNotification!.Topic,
                Guid.NewGuid().ToString(),
                new Notification<MessageDto>[]
                {
                    new()
                    {
                        RecipientId = request.RecipientId!,
                        Message = messageDto! with
                        {
                            IsMine = false
                        }
                    },
                    new()
                    {
                        RecipientId = request.SenderId!,
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
            request.Image == null ? Task.CompletedTask : _imageResizeRequestProducer.ProduceAsync(
                _kafkaSettings.ImageResizeRequest!.Topic,
                Guid.NewGuid().ToString(),
                new ImageResizeRequest
                {
                    MessageId = messageDto.Id,
                    ImageId = request.Image.Id
                }));

        await _messageCacheManager.RemoveAsync(request.SenderId!, request.RecipientId!);

        return new CreateMessageResponse
        {
            Dto = messageDto,
            Url = url
        };
    }
}
