﻿using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class EntityFactory : IEntityFactory
{
    private readonly LetsTalkDbContext _context;

    public EntityFactory(LetsTalkDbContext context)
    {
        _context = context;
    }

    public Message CreateMessage(int id)
    {
        var message = new Message(id);
        _context.Messages.Attach(message);

        return message;
    }

    public Domain.File CreateFile(int id)
    {
        var file = new Domain.File(id);
        _context.Files.Attach(file);

        return file;
    }

    public Domain.File CreateFile(string filename, FileTypes fileType)
    {
        return new Domain.File(filename, (int)fileType);
    }

    public Image CreateImage(ImageFormats imageFormat, ImageRoles imageRole, int width, int height)
    {
        return new Image((int)imageFormat, (int)imageRole, width, height);
    }

    public Account CreateAccount(string externalId, int accountTypeId, string firstName, string lastName, string email, string photoUrl)
    {
        return new Account(externalId, accountTypeId, firstName, lastName, email, photoUrl);
    }

    public LinkPreview CreateLinkPreview(string url, string title, string imageUrl)
    {
        return new LinkPreview(url, title, imageUrl);
    }
}