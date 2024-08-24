﻿using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.API.Core.Abstractions;

public interface IChatService
{
    Task<IReadOnlyList<ChatDto>> GetChatsAsync(string accountId, CancellationToken cancellationToken);
}
