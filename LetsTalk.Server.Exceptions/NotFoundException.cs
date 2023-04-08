namespace LetsTalk.Server.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key) : base($"{name} ({key}) was not found")
    {
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string? message) : base(message)
    {
    }

    public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
