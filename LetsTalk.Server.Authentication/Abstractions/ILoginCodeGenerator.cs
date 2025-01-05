namespace LetsTalk.Server.Authentication.Abstractions;

public interface ILoginCodeGenerator
{
    int GenerateCode();
}
