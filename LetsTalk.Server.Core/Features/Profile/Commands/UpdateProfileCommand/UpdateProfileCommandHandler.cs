using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(
        IAccountRepository accountRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
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

        var account = await _accountRepository.GetByIdAsTrackingAsync(request.AccountId!.Value, cancellationToken);
        account.UpdateProfile(request.FirstName!, request.LastName!, request.Email!, request.ImageId);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<AccountDto>(account);
    }
}
