using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LetsTalk.Server.Persistence.MongoDB.Models;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Text { get; protected set; }

    public string? TextHtml { get; protected set; }

    public string SenderId { get; protected set; }

    public string RecipientId { get; protected set; }

    public bool IsRead { get; protected set; }

    public long? DateCreatedUnix { get; protected set; }

    public long? DateReadUnix { get; protected set; }

    public int? ImageId { get; protected set; }

    public int? ImagePreviewId { get; protected set; }
}
