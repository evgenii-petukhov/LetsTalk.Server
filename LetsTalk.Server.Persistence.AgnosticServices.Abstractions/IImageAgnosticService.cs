using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IImageAgnosticService
{
    Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default);

    Task<ImageServiceModel?> GetByIdWithFileAsync(int id, CancellationToken cancellationToken = default);

    Task<ImageServiceModel> CreateImageAsync(
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default);

    Task<MessageServiceModel> SaveImagePreviewAsync(
        string messageId,
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default);
}
