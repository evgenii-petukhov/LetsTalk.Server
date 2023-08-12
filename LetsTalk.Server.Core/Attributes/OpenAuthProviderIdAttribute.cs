namespace LetsTalk.Server.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class OpenAuthProviderIdAttribute : BaseStringIdAttribute
{
    public OpenAuthProviderIdAttribute(string openAuthProviderId) : base(openAuthProviderId)
    {
    }
}
