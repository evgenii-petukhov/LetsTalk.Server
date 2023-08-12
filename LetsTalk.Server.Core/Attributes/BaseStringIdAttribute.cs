namespace LetsTalk.Server.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class BaseStringIdAttribute : Attribute
{
    public string Id { get; }

    protected BaseStringIdAttribute(string id)
    {
        Id = id;
    }
}
