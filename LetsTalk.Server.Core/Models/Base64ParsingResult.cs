using LetsTalk.Server.Core.Enums;

namespace LetsTalk.Server.Core.Models;

public class Base64ParsingResult
{
    public ImageTypes Imagetype { get; set; }

    public string? Base64string { get; set; }
}
