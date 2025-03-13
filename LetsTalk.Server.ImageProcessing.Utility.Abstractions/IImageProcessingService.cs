using LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;

namespace LetsTalk.Server.ImageProcessing.Utility.Abstractions;

public interface IImageProcessingService
{
    Task<ProcessImageResponse> ProcessImageAsync(string imageId, int maxWidth, int maxHeight, CancellationToken cancellationToken = default);
}
