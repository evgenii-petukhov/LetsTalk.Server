using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IBase64ParsingService
{
    string GetContentTypeName(ImageContentTypes contentType);
}
