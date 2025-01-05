namespace LetsTalk.Server.Authentication.Abstractions;

public interface ILoginCodeCacheService
{
    Task<(int, bool, TimeSpan)> GenerateCodeAsync(string email);

    Task<bool> ValidateCodeAsync(string email, int code);
}
