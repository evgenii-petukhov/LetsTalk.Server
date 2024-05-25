namespace LetsTalk.Server.API.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class OpenAuthProviderIdAttribute(string openAuthProviderId) : BaseStringIdAttribute<string>(openAuthProviderId)
{
}
