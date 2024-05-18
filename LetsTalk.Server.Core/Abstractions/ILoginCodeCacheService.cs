namespace LetsTalk.Server.Core.Abstractions;

public interface ILoginCodeCacheService
{
    Task<bool> GenerateCodeAsync(string email);

    Task<bool> ValidateCodeAsync(string email, int code);
}
