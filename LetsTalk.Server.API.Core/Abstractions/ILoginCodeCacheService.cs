namespace LetsTalk.Server.API.Core.Abstractions;

public interface ILoginCodeCacheService
{
    Task<(int, bool, TimeSpan)> GenerateCodeAsync(string email);

    Task<bool> ValidateCodeAsync(string email, int code);
}
