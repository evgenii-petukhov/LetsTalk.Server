namespace LetsTalk.Server.API.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class BaseStringIdAttribute<T>(T id) : Attribute
{
    public T Id { get; } = id;
}
