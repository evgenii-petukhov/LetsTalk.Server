using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageProducer _removeImageRequestProducer;
    private readonly KafkaSettings _kafkaSettings;

    public UpdateProfileCommandHandler(
        IAccountRepository accountRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _kafkaSettings = kafkaSettings.Value;
        _removeImageRequestProducer = producerAccessor.GetProducer(_kafkaSettings.MessageNotification!.Producer);
    }

    public async Task<AccountDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileCommandValidator(_accountRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var account = await _accountRepository.GetByIdAsTrackingAsync(request.AccountId!.Value, cancellationToken);
        var prevImageId = account.ImageId;
        account.UpdateProfile(request.FirstName!, request.LastName!, request.Email!, request.ImageId);

        await _unitOfWork.SaveAsync(cancellationToken);

        if (prevImageId.HasValue)
        {
            await _removeImageRequestProducer.ProduceAsync(
                _kafkaSettings.RemoveImageRequest!.Topic,
                Guid.NewGuid().ToString(),
                new RemoveImageRequest
                {
                    ImageId = prevImageId.Value
                });
        }

        return _mapper.Map<AccountDto>(account);
    }
}
