namespace LetsTalk.Server.FileStorage.Service.Abstractions;

public interface IImageCacheManager
{
    Task RemoveAsync(string imageId);
}
