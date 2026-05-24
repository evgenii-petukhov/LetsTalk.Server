using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Local.Services.Abstractions;

public interface IFileNameGenerator
{
    (string, string) Generate(FileTypes fileType);
}
