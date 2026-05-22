using LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.ImageProcessing.Utility.Abstractions;

public interface IImageProcessingService
{
    Task<ProcessImageResponse> ProcessImageAsync(
        string imageId,
        int maxWidth,
        int maxHeight,
        FileStorageTypes fileStorageTypeId,
        CancellationToken cancellationToken = default);
}
