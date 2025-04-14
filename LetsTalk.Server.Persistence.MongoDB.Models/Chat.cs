using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LetsTalk.Server.Persistence.MongoDB.Models;

public class Chat
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public Image? Image { get; set; }

    public bool IsIndividual { get; set; }

    public string? Name { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string>? AccountIds { get; set; }
}
