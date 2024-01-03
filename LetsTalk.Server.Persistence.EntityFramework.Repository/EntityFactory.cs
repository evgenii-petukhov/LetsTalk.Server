using LetsTalk.Server.Domain;
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

    public Image CreateImage(string id, ImageFormats imageFormat, int width, int height)
    {
        return new Image(id, (int)imageFormat, width, height);
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
