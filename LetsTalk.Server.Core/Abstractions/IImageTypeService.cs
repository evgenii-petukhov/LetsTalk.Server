using LetsTalk.Server.Core.Enums;
using LetsTalk.Server.Core.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IImageTypeService
{
    string GetExtensionByImageType(ImageTypes imageType);

    Base64ParsingResult? ParseBase64Image(string? input);
}
