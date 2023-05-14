using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IImageTypeService
{
    string GetExtensionByImageType(ImageContentTypes contentType);
}
