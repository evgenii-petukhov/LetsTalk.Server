using LetsTalk.Server.Core.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class LoginCodeGenerator() : ILoginCodeGenerator
{
    public int GenerateCode()
    {
        var rand = new Random(Guid.NewGuid().GetHashCode());
        return rand.Next(1000, 10000);
    }
}
