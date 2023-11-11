using LetsTalk.Server.Persistence.DatabaseAgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public class AccountEntityFrameworkService: IAccountDatabaseAgnosticService
{
    private readonly IAccountRepository _accountRepository;

    public AccountEntityFrameworkService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _accountRepository.IsAccountIdValidAsync(id, cancellationToken);
    }
}
