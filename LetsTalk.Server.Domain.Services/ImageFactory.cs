using LetsTalk.Server.Domain.Abstractions;

namespace LetsTalk.Server.Domain.Services;

public class ImageFactory : IImageFactory
{
    public Image CreateImage(int imageFormatId, int imageRoleId, int width, int height)
    {
        return new Image(imageFormatId, imageRoleId, width, height);
    }
}
