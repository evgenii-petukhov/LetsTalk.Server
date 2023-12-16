using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace LetsTalk.Server.Persistence.MongoDB.Models;

public abstract class Upload
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public File? File { get; set; }
}
