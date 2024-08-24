namespace LetsTalk.Server.API.Core.Abstractions;

public interface IOpenAuthProviderResolver<T>
{
    IOpenAuthProvider Resolve(T openAuthProviderId);
}
