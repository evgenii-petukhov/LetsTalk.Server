namespace LetsTalk.Server.FileStorage.Service.Abstractions;

public interface IImageCacheManager
{
    Task ClearAsync(string imageId);
}
