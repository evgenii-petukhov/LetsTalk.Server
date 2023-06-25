namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IImageCacheService
{
    void Add(int imageId, string filename);

    Task<string> GetOrAddAsync(int imageId, Func<int, Task<string>> predicate);

    void Remove(int imageId);
}
