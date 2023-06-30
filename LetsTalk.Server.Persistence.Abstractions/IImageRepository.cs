using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IImageRepository : IGenericRepository<Image>
{
    IQueryable<Image> GetById(int id);
}
