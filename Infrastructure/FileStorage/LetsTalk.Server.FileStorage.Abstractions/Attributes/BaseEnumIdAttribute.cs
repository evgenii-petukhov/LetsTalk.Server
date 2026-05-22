namespace LetsTalk.Server.FileStorage.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class BaseEnumIdAttribute<T>(T id) : Attribute
{
    public T Id { get; } = id;
}