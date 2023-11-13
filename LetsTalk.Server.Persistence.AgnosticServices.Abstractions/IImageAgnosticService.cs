using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IImageAgnosticService
{
    Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default);

    Task<ImageServiceModel?> GetByIdWithFileAsync(int id, CancellationToken cancellationToken = default);
}
