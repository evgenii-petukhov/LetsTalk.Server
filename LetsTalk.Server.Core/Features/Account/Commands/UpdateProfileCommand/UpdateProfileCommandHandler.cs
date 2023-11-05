using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountDomainService _accountDomainService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(
        IAccountRepository accountRepository,
        IAccountDomainService accountDomainService,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _accountDomainService = accountDomainService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileCommandValidator(_accountRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var account = await _accountDomainService.UpdateProfileAsync(
            request.AccountId!.Value,
            request.FirstName!,
            request.LastName!,
            request.Email!,
            request.ImageId,
            cancellationToken);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<AccountDto>(account);
    }
}
