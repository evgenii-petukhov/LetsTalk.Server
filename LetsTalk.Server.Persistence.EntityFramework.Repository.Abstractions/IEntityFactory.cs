using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IEntityFactory
{
    Message CreateMessage(int id);

    Image CreateImage(string id, ImageFormats imageFormat, int width, int height);

    Account CreateAccount(string externalId, int accountTypeId, string firstName, string lastName, string email, string photoUrl);

    LinkPreview CreateLinkPreview(string url, string title, string imageUrl);
}
