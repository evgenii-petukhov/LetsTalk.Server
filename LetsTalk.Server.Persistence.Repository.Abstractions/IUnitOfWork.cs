using System.Threading;
using System.Threading.Tasks;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken cancellationToken = default);
}
