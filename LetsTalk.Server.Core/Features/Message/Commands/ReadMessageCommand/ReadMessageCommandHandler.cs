using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;

public class ReadMessageCommandHandler : IRequestHandler<ReadMessageCommand, Unit>
{
    private readonly IEntityFactory _entityFactory;
    private readonly IUnitOfWork _unitOfWork;

    public ReadMessageCommandHandler(
        IEntityFactory entityFactory,
        IUnitOfWork unitOfWork)
    {
        _entityFactory = entityFactory;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ReadMessageCommand request, CancellationToken cancellationToken)
    {
        var message = _entityFactory.CreateMessage(request.MessageId);
        message.MarkAsRead();
        await _unitOfWork.SaveAsync(cancellationToken);
        return Unit.Value;
    }
}
