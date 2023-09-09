namespace LetsTalk.Server.Domain.Abstractions;

public interface IImageFactory
{
    Image CreateImage(int imageFormatId, int imageRoleId, int width, int height);
}
