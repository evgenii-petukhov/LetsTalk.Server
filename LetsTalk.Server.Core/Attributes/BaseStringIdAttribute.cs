namespace LetsTalk.Server.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class BaseStringIdAttribute<T> : Attribute
{
    public T Id { get; }

    protected BaseStringIdAttribute(T id)
    {
        Id = id;
    }
}
