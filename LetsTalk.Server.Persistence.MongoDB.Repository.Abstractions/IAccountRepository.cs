﻿using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

public interface IAccountRepository
{
    Task<Account> GetByExternalIdAsync(
        string externalId,
        AccountTypes accountType,
        CancellationToken cancellationToken = default);

    Task<Account> CreateAccountAsync(
        AccountTypes accountType,
        string email,
        CancellationToken cancellationToken);

    Task<Account> UpdateProfileAsync(
        string id,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task<Account> UpdateProfileAsync(
        string id,
        string firstName,
        string lastName,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        FileStorageTypes fileStorageType,
        CancellationToken cancellationToken = default);

    Task<Account> GetByEmailAsync(string email, AccountTypes accountType, CancellationToken cancellationToken = default);

    Task<Account> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<List<Account>> GetAccountsAsync(CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default);
}
