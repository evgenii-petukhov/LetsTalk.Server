using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IBase64ParsingService
{
    string? ParseBase64String(string? input);

    string CreateBase64String(byte[] content, ImageContentTypes contentType);
}
