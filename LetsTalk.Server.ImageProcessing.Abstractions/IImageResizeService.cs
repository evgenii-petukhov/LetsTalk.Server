namespace LetsTalk.Server.ImageProcessing.Abstractions;

public interface IImageResizeService
{
    byte[] Resize(byte[] data, int maxWidth, int maxHeight);
}
