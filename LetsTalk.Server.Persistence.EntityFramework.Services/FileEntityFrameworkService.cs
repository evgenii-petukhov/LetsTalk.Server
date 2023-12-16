using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class FileEntityFrameworkService : IFileAgnosticService
{
    private readonly IFileRepository _fileRepository;
    private readonly IEntityFactory _entityFactory;
    private readonly IUnitOfWork _unitOfWork;

    public FileEntityFrameworkService(
        IFileRepository fileRepository,
        IEntityFactory entityFactory,
        IUnitOfWork unitOfWork)
    {
        _fileRepository = fileRepository;
        _entityFactory = entityFactory;
        _unitOfWork = unitOfWork;
    }

    public Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var file = _entityFactory.CreateFile(int.Parse(id));
        _fileRepository.Delete(file);
        return _unitOfWork.SaveAsync(cancellationToken);
    }
}
