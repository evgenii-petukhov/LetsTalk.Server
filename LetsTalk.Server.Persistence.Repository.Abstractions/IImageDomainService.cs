﻿using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IImageDomainService
{
    Task<Image> CreateImageAsync(string filename, ImageFormats imageFormat, ImageRoles imageRole,
        int width, int height, CancellationToken cancellationToken);

    Task CreateImagePreviewAsync(string filename, ImageFormats imageFormat,
        int width, int height, int messageId);
}