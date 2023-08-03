using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IImageDataLayerService
{
    Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Image, T>> selector, CancellationToken cancellationToken = default);

    Task<int> CreateWithFileAsync(string filename, ImageFormats imageFormat, ImageRoles imageRole, int width, int height, CancellationToken cancellationToken = default);
}
