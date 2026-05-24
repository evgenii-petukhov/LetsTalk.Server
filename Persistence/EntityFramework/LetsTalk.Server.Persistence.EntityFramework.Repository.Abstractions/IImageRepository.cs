using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IImageRepository
{
    void Delete(Image image);
}
