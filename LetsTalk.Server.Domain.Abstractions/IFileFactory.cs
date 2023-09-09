namespace LetsTalk.Server.Domain.Abstractions;

public interface IFileFactory
{
    File CreateFile(string fileName, int fileTypeId);
}
