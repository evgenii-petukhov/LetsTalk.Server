using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LetsTalk.Server.Persistence.MongoDB.Models;

public class Chat
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ImageId { get; set; }

    public bool IsIndividual { get; set; }

    public string? Name { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string[]? AccountIds { get; set; }
}
