using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class ImageMongoDBService : IImageAgnosticService
{
    public Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ImageServiceModel?> GetByIdWithFileAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ImageServiceModel> CreateImageAsync(
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageServiceModel> SaveImagePreviewAsync(
        string messageId,
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
