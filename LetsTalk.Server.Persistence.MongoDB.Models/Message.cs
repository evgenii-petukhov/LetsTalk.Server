using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LetsTalk.Server.Persistence.MongoDB.Models;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Text { get; set; }

    public string? TextHtml { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? SenderId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? RecipientId { get; set; }

    public bool IsRead { get; set; }

    public long? DateCreatedUnix { get; set; }

    public long? DateReadUnix { get; set; }

    public int? ImageId { get; set; }

    public int? ImagePreviewId { get; set; }
}
