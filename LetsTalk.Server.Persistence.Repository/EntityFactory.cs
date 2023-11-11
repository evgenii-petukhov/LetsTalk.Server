using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository;

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

    public Domain.File CreateFile(string filename, FileTypes fileType)
    {
        var file = new Domain.File(filename, (int)fileType);
        _context.Files.Attach(file);

        return file;
    }

    public Domain.Image CreateImage(ImageFormats imageFormat, ImageRoles imageRole, int width, int height)
    {
        var image = new Domain.Image((int)imageFormat, (int)imageRole, width, height);
        _context.Images.Attach(image);

        return image;
    }

    public Account CreateAccount(string externalId, int accountTypeId, string firstName, string lastName, string email, string photoUrl)
    {
        var account = new Account(externalId, accountTypeId, firstName, lastName, email, photoUrl);
        _context.Accounts.Attach(account);

        return account;
    }

    public LinkPreview CreateLinkPreview(string url, string title, string imageUrl)
    {
        var linkPreview = new LinkPreview(url, title, imageUrl);
        _context.LinkPreviews.Attach(linkPreview);

        return linkPreview;
    }
}
