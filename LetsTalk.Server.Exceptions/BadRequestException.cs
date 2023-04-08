﻿namespace LetsTalk.Server.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException()
    {
    }

    public BadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
