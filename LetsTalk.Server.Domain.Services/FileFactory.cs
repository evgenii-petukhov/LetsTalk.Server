using LetsTalk.Server.Domain.Abstractions;

namespace LetsTalk.Server.Domain.Services;

public class FileFactory : IFileFactory
{
    public File CreateFile(string fileName, int fileTypeId)
    {
        return new File(fileName, fileTypeId);
    }
}
