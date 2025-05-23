﻿using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class EntityFactory(LetsTalkDbContext context) : IEntityFactory
{
    private readonly LetsTalkDbContext _context = context;

    public Image CreateImage(string id, ImageFormats imageFormat, int width, int height, FileStorageTypes fileStorageType)
    {
        return new Image(id, (int)imageFormat, width, height, (int)fileStorageType);
    }

    public Account CreateAccount(int accountTypeId, string email)
    {
        return new Account(accountTypeId, email);
    }

    public LinkPreview CreateLinkPreview(string url, string title, string imageUrl)
    {
        return new LinkPreview(url, title, imageUrl);
    }

    public ChatMessageStatus CreateChatMessageStatus(int chatId, int accountId, int messageId, bool attachToContext = false)
    {
        var chatMessageStatus = new ChatMessageStatus(chatId, accountId, messageId);

        if (attachToContext)
        {
            _context.ChatMessageStatuses.Attach(chatMessageStatus);
        }

        return chatMessageStatus;
    }
}
