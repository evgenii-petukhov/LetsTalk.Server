using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Attributes;
using System.Reflection;

namespace LetsTalk.Server.Core.Services;

public class OpenAuthProviderResolver<TAttribute> : IOpenAuthProviderResolver
     where TAttribute : BaseStringIdAttribute
{
    private readonly IReadOnlyDictionary<string, IOpenAuthProvider> _openAuthProviderMapping;

    public OpenAuthProviderResolver(
        IEnumerable<IOpenAuthProvider> openAuthProviders)
    {
        _openAuthProviderMapping = GetOpenAuthProviderMapping(openAuthProviders);
    }

    public IOpenAuthProvider Resolve(string openAuthProviderId)
    {
        return _openAuthProviderMapping.GetValueOrDefault(openAuthProviderId)!;
    }

    private static IReadOnlyDictionary<string, IOpenAuthProvider> GetOpenAuthProviderMapping(IEnumerable<IOpenAuthProvider> openAuthProviders)
    {
        return openAuthProviders
            .Select(provider => new
            {
                Attribute = provider.GetType().GetCustomAttribute<TAttribute>(),
                Provider = provider
            })
            .Where(t => t.Attribute != null)
            .ToDictionary(t => t.Attribute!.Id, t => t.Provider);
    }
}
