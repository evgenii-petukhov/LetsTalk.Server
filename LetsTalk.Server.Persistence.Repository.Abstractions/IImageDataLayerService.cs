using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IImageDataLayerService
{
    Task<Image> CreateImageAsync(string filename, ImageFormats imageFormat, ImageRoles imageRole,
        int width, int height, CancellationToken cancellationToken = default);

    Task<Image> CreateImagePreviewAsync(string filename, ImageFormats imageFormat,
        int width, int height, int messageId, CancellationToken cancellationToken = default);

    Task<Image> ReplaceImageAsync(string filename, ImageFormats imageFormat, ImageRoles imageRole,
        int width, int height, int prevImageId, CancellationToken cancellationToken = default);
}
