namespace LetsTalk.Server.Authentication.Abstractions;

public interface ILoginCodeCacheService
{
    ValueTask<(int, bool, TimeSpan)> GenerateCodeAsync(string email);

    Task<bool> ValidateCodeAsync(string email, int code);
}
