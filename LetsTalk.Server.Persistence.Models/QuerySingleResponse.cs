namespace LetsTalk.Server.Persistence.Models;

public class QuerySingleResponse<T>
{
    public QuerySingleResponse(T value)
    {
        Value = value;
        HasValue = true;
    }

    public QuerySingleResponse()
    {
        HasValue = false;
    }

    public bool HasValue { get; init; }

    public T? Value { get; init; }
}
