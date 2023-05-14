using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Core.Models;

public class Base64ParsingResult
{
    public ImageFileTypes ImageFileType { get; set; }

    public string? Base64string { get; set; }
}
