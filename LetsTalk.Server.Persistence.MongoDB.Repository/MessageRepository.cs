﻿using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using LetsTalk.Server.Utility.Common;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class MessageRepository : IMessageRepository
{
    private static readonly ReplaceOptions UpsertReplaceOptions = new()
    {
        IsUpsert = true
    };

    private readonly IMongoCollection<Message> _messageCollection;
    private readonly IMongoCollection<ChatMessageStatus> _chatMessageStatusCollection;
    private readonly IMongoCollection<LinkPreview> _linkPreviewCollection;

    public MessageRepository(
        IMongoClient mongoClient,
        IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);

        _messageCollection = mongoDatabase.GetCollection<Message>(nameof(Message));
        _chatMessageStatusCollection = mongoDatabase.GetCollection<ChatMessageStatus>(nameof(ChatMessageStatus));
        _linkPreviewCollection = mongoDatabase.GetCollection<LinkPreview>(nameof(LinkPreview));
    }

    public Task<List<Message>> GetPagedAsync(string chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default)
    {
        return _messageCollection
            .AsQueryable()
            .Where(message => message.ChatId == chatId)
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
        string chatId,
        string text,
        string textHtml,
        string linkPreviewId,
        CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            SenderId = senderId,
            ChatId = chatId,
            Text = text,
            TextHtml = textHtml,
            LinkPreviewId = linkPreviewId,
            DateCreatedUnix = DateHelper.GetUnixTimestamp()
        };

        await _messageCollection.InsertOneAsync(message, cancellationToken: cancellationToken);

        return message;
    }

    public async Task<Message> CreateAsync(
        string senderId,
        string chatId,
        string text,
        string textHtml,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        FileStorageTypes fileStorageType,
        CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            SenderId = senderId,
            ChatId = chatId,
            Text = text,
            TextHtml = textHtml,
            DateCreatedUnix = DateHelper.GetUnixTimestamp(),
            Image = new Image
            {
                Id = imageId,
                ImageFormatId = (int)imageFormat,
                Width = width,
                Height = height,
                FileStorageTypeId = (int)fileStorageType
            }
        };

        await _messageCollection.InsertOneAsync(message, cancellationToken: cancellationToken);

        return message;
    }

    public Task<Message> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _messageCollection.Aggregate()
            .Match(Builders<Message>.Filter.Eq(x => x.Id, id))
            .Lookup<Message, LinkPreview, Message>(
                foreignCollection: _linkPreviewCollection,
                localField: m => m.LinkPreviewId,
                foreignField: lp => lp.Id,
                @as: m => m.LinkPreview)
            .Unwind(m => m.LinkPreview, new AggregateUnwindOptions<Message>
            {
                PreserveNullAndEmptyArrays = true
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task MarkAsReadAsync(string chatId, string accountId, string messageId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ChatMessageStatus>.Filter.And(
            Builders<ChatMessageStatus>.Filter.Eq(x => x.ChatId, chatId),
            Builders<ChatMessageStatus>.Filter.Eq(x => x.AccountId, accountId),
            Builders<ChatMessageStatus>.Filter.Eq(x => x.MessageId, messageId)
        );

        var chatMessageStatus = new ChatMessageStatus
        {
            ChatId = chatId,
            AccountId = accountId,
            MessageId = messageId,
            DateReadUnix = DateHelper.GetUnixTimestamp()
        };

        return _chatMessageStatusCollection.ReplaceOneAsync(filter, chatMessageStatus, UpsertReplaceOptions, cancellationToken);
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
        FileStorageTypes fileStorageType,
        CancellationToken cancellationToken = default)
    {
        return _messageCollection.FindOneAndUpdateAsync(
            Builders<Message>.Filter.Eq(x => x.Id, messageId),
            Builders<Message>.Update.Set(x => x.ImagePreview, new Image
            {
                Id = filename,
                ImageFormatId = (int)imageFormat,
                Width = width,
                Height = height,
                FileStorageTypeId = (int)fileStorageType
            }),
            new FindOneAndUpdateOptions<Message, Message>
            {
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken: cancellationToken);
    }
}
