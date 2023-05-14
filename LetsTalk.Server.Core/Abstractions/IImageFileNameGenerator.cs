using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IImageFileNameGenerator
{
    string GetFilename(ImageFileTypes imageFileType);
}
