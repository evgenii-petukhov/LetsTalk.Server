namespace LetsTalk.Server.Core.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {

    }
}
