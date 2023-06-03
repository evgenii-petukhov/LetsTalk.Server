using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IImageTypeService
{
    string GetExtensionByImageType(ImageContentTypes contentType);
}
