namespace LetsTalk.Server.FileStorage.Service.Abstractions;

public interface IImageStorageCacheManager
{
    Task ClearAsync(string imageId);
}
