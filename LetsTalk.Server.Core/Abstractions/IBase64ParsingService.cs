using LetsTalk.Server.Core.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IBase64ParsingService
{
    Base64ParsingResult? ParseBase64Image(string? input);
}
