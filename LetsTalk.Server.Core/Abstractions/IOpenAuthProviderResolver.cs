namespace LetsTalk.Server.Core.Abstractions;

public interface IOpenAuthProviderResolver<T>
{
    IOpenAuthProvider Resolve(T openAuthProviderId);
}
