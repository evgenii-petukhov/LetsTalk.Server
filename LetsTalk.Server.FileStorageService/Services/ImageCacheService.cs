using LetsTalk.Server.FileStorageService.Abstractions;
using System.Collections.Concurrent;

namespace LetsTalk.Server.FileStorageService.Services;

public class ImageCacheService : IImageCacheService
{
    private readonly ConcurrentDictionary<int, Task<string>> _cache = new();

    public void Add(int imageId, string filename)
    {
        _cache.TryAdd(imageId, Task.FromResult(filename));
    }

    public Task<string> GetOrAddAsync(int imageId, Func<int, Task<string>> predicate)
    {
        return _cache.GetOrAdd(imageId, predicate);
    }

    public void Remove(int imageId)
    {
        _cache.TryRemove(imageId, out _);
    }
}
