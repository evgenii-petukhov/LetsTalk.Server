﻿using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility.Abstractions;

public interface IImageService
{
    Task<int> SaveImageAsync(byte[] content, ImageRoles imageRole, ImageFormats imageFormat, CancellationToken cancellationToken = default);

    Task<byte[]> FetchImageAsync(int imageId, CancellationToken cancellationToken = default);
}