using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Services;

public class ImageDataLayerService : IImageDataLayerService
{
    private readonly IImageRepository _imageRepository;

    public ImageDataLayerService(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    public Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Image, T>> selector, CancellationToken cancellationToken = default)
    {
        return _imageRepository.GetById(id)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}
