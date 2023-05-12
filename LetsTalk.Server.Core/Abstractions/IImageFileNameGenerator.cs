using LetsTalk.Server.Core.Enums;

namespace LetsTalk.Server.Core.Abstractions;

public interface IImageFileNameGenerator
{
    string GetFilename(ImageTypes imageType);
}
