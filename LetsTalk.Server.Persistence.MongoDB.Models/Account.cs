using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LetsTalk.Server.Persistence.MongoDB.Models;

public class Account
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public int AccountTypeId { get; set; }

    public string? ExternalId { get; set; }

    public string? Email { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ImageId { get; set; }
}
