﻿using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileDto>
{
    private readonly IAccountAgnosticService _accountAgnosticService;
    private readonly IMapper _mapper;
    private readonly IProfileCacheManager _profileCacheManager;
    private readonly IMessageProducer _producer;
    private readonly KafkaSettings _kafkaSettings;

    public UpdateProfileCommandHandler(
        IAccountAgnosticService accountAgnosticService,
        IMapper mapper,
        IProfileCacheManager profileCacheManager,
        IOptions<KafkaSettings> kafkaSettings,
        IProducerAccessor producerAccessor)
    {
        _accountAgnosticService = accountAgnosticService;
        _mapper = mapper;
        _profileCacheManager = profileCacheManager;
        _kafkaSettings = kafkaSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.MessageNotification!.Producer);
    }

    public async Task<ProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountAgnosticService.GetByIdAsync(request.AccountId!, cancellationToken);

        var previousImageId = account.ImageId;
        var deletePreviousImage = previousImageId != null && request.Image != null;

        account = request.Image == null
            ? await _accountAgnosticService.UpdateProfileAsync(
                request.AccountId!,
                request.FirstName!,
                request.LastName!,
                cancellationToken)
            : await _accountAgnosticService.UpdateProfileAsync(
                request.AccountId!,
                request.FirstName!,
                request.LastName!,
                request.Image.Id!,
                request.Image.Width,
                request.Image.Height,
                (ImageFormats)request.Image.ImageFormat,
                cancellationToken);

        await _profileCacheManager.ClearAsync(request.AccountId!);

        if (deletePreviousImage)
        {
            await _producer.ProduceAsync(
                _kafkaSettings.RemoveImageRequest!.Topic,
                Guid.NewGuid().ToString(),
                new RemoveImageRequest
                {
                    ImageId = previousImageId
                });
        }

        return _mapper.Map<ProfileDto>(account);
    }
}
