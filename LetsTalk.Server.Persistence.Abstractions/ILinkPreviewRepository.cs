﻿using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface ILinkPreviewRepository : IGenericRepository<LinkPreview>
{
    Task<LinkPreview?> GetByUrlOrDefaultAsync(string url, CancellationToken cancellationToken = default);
}
