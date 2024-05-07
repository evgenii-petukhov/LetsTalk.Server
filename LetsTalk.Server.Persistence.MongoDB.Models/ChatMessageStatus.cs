using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LetsTalk.Server.Persistence.MongoDB.Models;

public class ChatMessageStatus
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ChatId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? AccountId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? MessageId { get; set; }

    public long? DateReadUnix { get; set; }
}
