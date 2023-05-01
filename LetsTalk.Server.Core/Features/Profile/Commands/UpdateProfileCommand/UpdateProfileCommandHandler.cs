using LetsTalk.Server.API.Models.UpdateProfile;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IAccountRepository _accountRepository;

    public UpdateProfileCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        return _accountRepository.UpdateAsync(request.AccountId, request.FirstName, request.LastName, request.Email);
    }
}
