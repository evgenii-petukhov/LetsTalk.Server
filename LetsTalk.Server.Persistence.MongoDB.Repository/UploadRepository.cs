using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class UploadRepository : IUploadRepository
{
    private readonly IMongoCollection<Upload> _uploadCollection;

    public UploadRepository(
        IMongoClient mongoClient,
        IOptions<DatabaseSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.MongoDatabaseName);

        _uploadCollection = mongoDatabase.GetCollection<Upload>(nameof(Upload));
    }

    public async Task<Image> CreateImageAsync(
        string filename,
        ImageFormats imageFormat,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var image = new Image
        {
            File = new Models.File
            {
                FileName = filename,
                FileTypeId = (int)FileTypes.Image
            },
            ImageFormatId = (int)imageFormat,
            Width = width,
            Height = height
        };

        await _uploadCollection.InsertOneAsync(image, cancellationToken: cancellationToken);

        return image;
    }
}
