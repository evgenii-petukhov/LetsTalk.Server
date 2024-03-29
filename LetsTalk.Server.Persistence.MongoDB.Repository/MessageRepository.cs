﻿using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using LetsTalk.Server.Persistence.Utility;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class MessageRepository : IMessageRepository
{
    private readonly IMongoCollection<Message> _messageCollection;
    private readonly IMongoCollection<LinkPreview> _linkPreviewCollection;

    public MessageRepository(
        IMongoClient mongoClient,
        IOptions<DatabaseSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.MongoDatabaseName);

        _messageCollection = mongoDatabase.GetCollection<Message>(nameof(Message));
        _linkPreviewCollection = mongoDatabase.GetCollection<LinkPreview>(nameof(LinkPreview));
    }

    public Task<List<Message>> GetPagedAsync(string senderId, string recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default)
    {
        return _messageCollection
            .AsQueryable()
            .Where(message => (message.SenderId == senderId && message.RecipientId == recipientId) || (message.SenderId == recipientId && message.RecipientId == senderId))
            .GroupJoin(_linkPreviewCollection, x => x.LinkPreviewId, x => x.Id, (message, linkPreviews) => new
            {
                Message = message,
                LinkPreviews = linkPreviews
            })
            .SelectMany(x => x.LinkPreviews.DefaultIfEmpty(), (g, linkPreview) => new Message
            {
                Id = g.Message.Id,
                Text = g.Message.Text,
                TextHtml = g.Message.TextHtml,
                SenderId = g.Message.SenderId,
                RecipientId = g.Message.RecipientId,
                IsRead = g.Message.IsRead,
                DateCreatedUnix = g.Message.DateCreatedUnix,
                LinkPreview = linkPreview,
                Image = g.Message.Image,
                ImagePreview = g.Message.ImagePreview
            })
            .OrderByDescending(mesage => mesage.DateCreatedUnix)
            .Skip(messagesPerPage * pageIndex)
            .Take(messagesPerPage)
            .OrderBy(mesage => mesage.DateCreatedUnix)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message> CreateAsync(
        string senderId,
        string recipientId,
        string text,
        string textHtml,
        CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            SenderId =  senderId,
            RecipientId = recipientId,
            Text = text,
            TextHtml = textHtml,
            DateCreatedUnix = DateHelper.GetUnixTimestamp()
        };

        await _messageCollection.InsertOneAsync(message, cancellationToken: cancellationToken);

        return message;
    }

    public async Task<Message> CreateAsync(
        string senderId,
        string recipientId,
        string text,
        string textHtml,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            SenderId = senderId,
            RecipientId = recipientId,
            Text = text,
            TextHtml = textHtml,
            DateCreatedUnix = DateHelper.GetUnixTimestamp(),
            Image = new Image
            {
                Id = imageId,
                ImageFormatId = (int)imageFormat,
                Width = width,
                Height = height
            }
        };

        await _messageCollection.InsertOneAsync(message, cancellationToken: cancellationToken);

        return message;
    }

    public Task<Message> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _messageCollection
            .Find(Builders<Message>.Filter.Eq(x => x.Id, id))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task MarkAsReadAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return _messageCollection.UpdateOneAsync(
            Builders<Message>.Filter.Eq(x => x.Id, messageId),
            Builders<Message>.Update
                .Set(x => x.IsRead, true)
                .Set(x => x.DateReadUnix, DateHelper.GetUnixTimestamp()),
            cancellationToken: cancellationToken);
    }

    public Task MarkAllAsReadAsync(string senderId, string recipientId, long dateCreatedUnix, CancellationToken cancellationToken = default)
    {
        return _messageCollection.UpdateManyAsync(
            Builders<Message>.Filter.Where(message => message.DateCreatedUnix <= dateCreatedUnix && message.SenderId == senderId && message.RecipientId == recipientId && !message.IsRead),
            Builders<Message>.Update
                .Set(x => x.IsRead, true)
                .Set(x => x.DateReadUnix, DateHelper.GetUnixTimestamp()),
            cancellationToken: cancellationToken);
    }

    public async Task<Message> SetLinkPreviewAsync(string messageId, string linkPreviewId, CancellationToken cancellationToken = default)
    {
        var message = await _messageCollection.FindOneAndUpdateAsync(
            Builders<Message>.Filter.Eq(x => x.Id, messageId),
            Builders<Message>.Update.Set(x => x.LinkPreviewId, linkPreviewId),
            new FindOneAndUpdateOptions<Message, Message>
            {
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken: cancellationToken);

        message.LinkPreview = await _linkPreviewCollection
            .Find(Builders<LinkPreview>.Filter.Eq(x => x.Id, linkPreviewId))
            .FirstOrDefaultAsync(cancellationToken);

        return message;
    }

    public async Task<Message> SetLinkPreviewAsync(
        string messageId,
        string url,
        string title,
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        var linkPreview = new LinkPreview
        {
            Url = url,
            Title = title,
            ImageUrl = imageUrl
        };

        await _linkPreviewCollection.InsertOneAsync(linkPreview, cancellationToken: cancellationToken);

        var message = await _messageCollection.FindOneAndUpdateAsync(
            Builders<Message>.Filter.Eq(x => x.Id, messageId),
            Builders<Message>.Update.Set(x => x.LinkPreviewId, linkPreview.Id),
            new FindOneAndUpdateOptions<Message, Message>
            {
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken: cancellationToken);

        message.LinkPreview = linkPreview;

        return message;
    }

    public Task<Message> SetImagePreviewAsync(
        string messageId,
        string filename,
        ImageFormats imageFormat,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        return _messageCollection.FindOneAndUpdateAsync(
            Builders<Message>.Filter.Eq(x => x.Id, messageId),
            Builders<Message>.Update.Set(x => x.ImagePreview, new Image
            {
                Id = filename,
                ImageFormatId = (int)imageFormat,
                Width = width,
                Height = height
            }),
            new FindOneAndUpdateOptions<Message, Message>
            {
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken: cancellationToken);
    }
}
