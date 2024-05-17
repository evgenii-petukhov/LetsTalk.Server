using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IEntityFactory
{
    Image CreateImage(string id, ImageFormats imageFormat, int width, int height);

    Account CreateAccount(string externalId, int accountTypeId, string firstName, string lastName, string photoUrl);

    Account CreateAccount(int accountTypeId, string email);

    LinkPreview CreateLinkPreview(string url, string title, string imageUrl);

    ChatMessageStatus CreateChatMessageStatus(int chatId, int accountId, int messageId, bool attachToContext = false);
}
