using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Attributes;
using System.Reflection;

namespace LetsTalk.Server.API.Core.Services;

public class OpenAuthProviderResolver<TKey, TAttribute>(
    IEnumerable<IOpenAuthProvider> openAuthProviders) : IOpenAuthProviderResolver<TKey>
     where TKey: notnull
     where TAttribute : BaseStringIdAttribute<TKey>
{
    private readonly Dictionary<TKey, IOpenAuthProvider> _openAuthProviderMapping = GetOpenAuthProviderMapping(openAuthProviders);

    public IOpenAuthProvider Resolve(TKey openAuthProviderId)
    {
        return _openAuthProviderMapping.GetValueOrDefault(openAuthProviderId)!;
    }

    private static Dictionary<TKey, IOpenAuthProvider> GetOpenAuthProviderMapping(IEnumerable<IOpenAuthProvider> openAuthProviders)
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
