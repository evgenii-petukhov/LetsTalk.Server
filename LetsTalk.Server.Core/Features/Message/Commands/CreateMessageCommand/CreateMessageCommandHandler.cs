using AutoMapper;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, CreateMessageResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IMessageProcessor _messageProcessor;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMessageCommandHandler(
        IAccountRepository accountRepository,
        IMessageRepository messageRepository,
        IImageRepository imageRepository,
        IMessageProcessor messageProcessor,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _messageRepository = messageRepository;
        _messageProcessor = messageProcessor;
        _imageRepository = imageRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
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
        return new CreateMessageResponse
        {
            Dto = _mapper.Map<MessageDto>(message),
            Url = url
        };
    }
}
