using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public UpdateProfileCommandHandler(
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<AccountDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileCommandValidator(_accountRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        await _accountRepository.UpdateAsync(request.AccountId!.Value, request.FirstName, request.LastName, request.Email, cancellationToken);
        var account = await _accountRepository.GetByIdAsync(request.AccountId!.Value, cancellationToken);

        return _mapper.Map<AccountDto>(account);
    }
}
