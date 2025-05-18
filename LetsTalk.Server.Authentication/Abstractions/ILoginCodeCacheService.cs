namespace LetsTalk.Server.Authentication.Abstractions;

public interface ILoginCodeCacheService
{
    ValueTask<(int, bool, TimeSpan)> GenerateCodeAsync(string email);

    ValueTask<bool> ValidateCodeAsync(string email, int code);
}
