using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class ImageRepository : IImageRepository
{
    private readonly IMongoCollection<Upload> _uploadCollection;

    public ImageRepository(
        IMongoClient mongoClient,
        IOptions<DatabaseSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.MongoDatabaseName);

        _uploadCollection = mongoDatabase.GetCollection<Upload>(nameof(Upload));
    }

    public Task<bool> IsImageIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _uploadCollection
            .Find(Builders<Upload>.Filter.OfType<Image>() & Builders<Upload>.Filter.Eq(x => x.Id, id))
            .AnyAsync(cancellationToken);
    }

    public async Task<Image> CreateImageAsync(
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
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
            ImageRoleId = (int)imageRole,
            Width = width,
            Height = height
        };

        await _uploadCollection.InsertOneAsync(image, cancellationToken: cancellationToken);

        return image;
    }

    public Task<Image?> GetByIdWithFileAsync(string id, CancellationToken cancellationToken = default)
    {
        return _uploadCollection
            .Find(Builders<Upload>.Filter.OfType<Image>() & Builders<Upload>.Filter.Eq(x => x.Id, id))
            .Project(Builders<Upload>.Projection.As<Image>())
            .FirstOrDefaultAsync(cancellationToken)!;
    }
}
