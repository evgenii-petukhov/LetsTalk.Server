using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Attributes;
using System.Reflection;

namespace LetsTalk.Server.Core.Services;

public class OpenAuthProviderResolver<TKey, TAttribute> : IOpenAuthProviderResolver<TKey>
     where TKey: notnull
     where TAttribute : BaseStringIdAttribute<TKey>
{
    private readonly IReadOnlyDictionary<TKey, IOpenAuthProvider> _openAuthProviderMapping;

    public OpenAuthProviderResolver(
        IEnumerable<IOpenAuthProvider> openAuthProviders)
    {
        _openAuthProviderMapping = GetOpenAuthProviderMapping(openAuthProviders);
    }

    public IOpenAuthProvider Resolve(TKey openAuthProviderId)
    {
        return _openAuthProviderMapping.GetValueOrDefault(openAuthProviderId)!;
    }

    private static IReadOnlyDictionary<TKey, IOpenAuthProvider> GetOpenAuthProviderMapping(IEnumerable<IOpenAuthProvider> openAuthProviders)
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
