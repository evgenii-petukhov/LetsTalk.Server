﻿using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IContactsService
{
    Task<List<AccountDto>> GetContactsAsync(string accountId, CancellationToken cancellationToken);
}
