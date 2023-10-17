using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IEntityFactory
{
    Message CreateMessage(int id);

    Domain.File CreateFile(string filename, FileTypes fileType);

    Image CreateImage(ImageFormats imageFormat, ImageRoles imageRole, int width, int height);

    Account CreateAccount(string externalId, int accountTypeId, string firstName, string lastName, string email, string photoUrl);

    LinkPreview CreateLinkPreview(string url, string title, string imageUrl);
}
