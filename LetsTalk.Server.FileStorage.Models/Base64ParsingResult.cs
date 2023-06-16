using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorage.Models;

public class Base64ParsingResult
{
    public ImageContentTypes ImageContentType { get; set; }

    public string? Base64string { get; set; }
}
