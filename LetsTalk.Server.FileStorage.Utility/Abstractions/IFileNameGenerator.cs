using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility.Abstractions;

public interface IFileNameGenerator
{
    (string, string) Generate(FileTypes fileType);
}
