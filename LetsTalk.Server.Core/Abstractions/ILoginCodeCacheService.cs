namespace LetsTalk.Server.Core.Abstractions;

public interface ILoginCodeCacheService
{
    Task<(int, bool)> GenerateCodeAsync(string email);

    Task<bool> ValidateCodeAsync(string email, int code);
}
