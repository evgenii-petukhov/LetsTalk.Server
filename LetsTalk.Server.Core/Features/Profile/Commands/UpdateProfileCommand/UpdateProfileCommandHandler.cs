using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IMapper _mapper;

    public UpdateProfileCommandHandler(
        IAccountRepository accountRepository,
        IAccountDataLayerService accountDataLayerService,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _accountDataLayerService = accountDataLayerService;
        _mapper = mapper;
    }

    public async Task<AccountDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileCommandValidator(_accountDataLayerService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        await _accountRepository.UpdateAsync(request.AccountId!.Value, request.FirstName, request.LastName, request.Email, cancellationToken);
        var account = await _accountDataLayerService.GetByIdAsync(request.AccountId!.Value, cancellationToken: cancellationToken);

        return _mapper.Map<AccountDto>(account);
    }
}
