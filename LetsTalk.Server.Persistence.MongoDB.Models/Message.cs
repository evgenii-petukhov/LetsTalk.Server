﻿using MongoDB.Bson;
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
    public string? ChatId { get; set; }

    public long? DateCreatedUnix { get; set; }

    public Image? Image { get; set; }

    public Image? ImagePreview { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? LinkPreviewId { get; set; }

    public LinkPreview? LinkPreview { get; set; }
}
