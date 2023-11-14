using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class AccountMongoDBService : IAccountAgnosticService
{
    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountServiceModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountServiceModel> UpdateProfileAsync(
        int accountId,
        string firstName,
        string lastName,
        string email,
        int? imageId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateOrUpdateAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ContactServiceModel>> GetContactsAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
