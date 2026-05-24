namespace LetsTalk.Server.FileStorage.Service.Exceptions;

public class ImageSizeException : Exception
{
    public ImageSizeException()
    {
    }

    public ImageSizeException(string? message) : base(message)
    {
    }

    public ImageSizeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
