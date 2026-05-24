using FluentValidation.Results;

namespace LetsTalk.Server.Exceptions;

public class BadRequestException : Exception
{
    public IDictionary<string, string[]>? ValidationErrors { get; }

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException()
    {
    }

    public BadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public BadRequestException(string message, ValidationResult validationResult) : base(message)
    {
        ValidationErrors = validationResult.ToDictionary();
    }
}
