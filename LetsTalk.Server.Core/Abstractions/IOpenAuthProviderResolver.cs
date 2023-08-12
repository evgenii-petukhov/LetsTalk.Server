using LetsTalk.Server.Core.Attributes;

namespace LetsTalk.Server.Core.Abstractions;

public interface IOpenAuthProviderResolver
{
    IOpenAuthProvider Resolve(string openAuthProviderId);
}
