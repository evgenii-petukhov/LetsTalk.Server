using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Amazon.Services;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.ImageProcessing.Lambda;

public class FakeFileServiceResolver(string bucketName) : IFileServiceResolver
{
    private readonly string _bucketName = bucketName;

    public IFileService Resolve()
    {
        throw new NotImplementedException();
    }

    public IFileService Resolve(FileStorageTypes fileStorageType)
    {
        return new AmazonFileService(_bucketName);
    }
}
