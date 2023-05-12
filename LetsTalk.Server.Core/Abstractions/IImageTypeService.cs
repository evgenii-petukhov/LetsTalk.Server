using LetsTalk.Server.Core.Enums;

namespace LetsTalk.Server.Core.Abstractions;

public interface IImageTypeService
{
    string GetExtensionByImageType(ImageTypes imageType);
}
