using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IEntityFactory
{
    Domain.Message CreateMessage(int id);

    Domain.File CreateFile(string filename, FileTypes fileType);

    Domain.Image CreateImage(ImageFormats imageFormat, ImageRoles imageRole, int width, int height);
}
