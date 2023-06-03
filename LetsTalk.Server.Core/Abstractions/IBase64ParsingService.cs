using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IBase64ParsingService
{
    Base64ParsingResult? ParseBase64String(string? input);

    string CreateBase64String(byte[] content, ImageContentTypes contentType);
}
