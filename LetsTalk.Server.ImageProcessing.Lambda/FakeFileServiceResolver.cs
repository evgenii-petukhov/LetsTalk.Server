using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Amazon.Services;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.ImageProcessing.Lambda;

public class FakeFileServiceResolver(string bucketName) : IFileServiceResolver
{
    private readonly string _bucketName = bucketName;

    public IFileService Resolve()
    {
        return new AmazonFileService(_bucketName);
    }

    public IFileService Resolve(FileStorageTypes fileStorageType)
    {
        throw new NotImplementedException();
    }
}
