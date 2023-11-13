using AutoMapper;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public class AccountEntityFrameworkService: IAccountAgnosticService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountEntityFrameworkService(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _accountRepository.IsAccountIdValidAsync(id, cancellationToken);
    }

    public async Task<AccountServiceModel> UpdateProfileAsync(
        int accountId,
        string firstName,
        string lastName,
        string email,
        int? imageId,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsTrackingAsync(accountId, cancellationToken);
        account.UpdateProfile(firstName, lastName, email, imageId);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<AccountServiceModel>(account);
    }
}
