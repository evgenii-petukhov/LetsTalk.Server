using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;

public class ReadMessageCommandHandler : IRequestHandler<ReadMessageCommand, Unit>
{
    private readonly IMessageDomainService _messageDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public ReadMessageCommandHandler(
        IMessageDomainService messageDomainService,
        IUnitOfWork unitOfWork)
    {
        _messageDomainService = messageDomainService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ReadMessageCommand request, CancellationToken cancellationToken)
    {
        if (request.UpdatePreviousMessages)
        {
            await _messageDomainService.MarkAllAsRead(request.RecipientId, request.MessageId);
        }
        else
        {
            _messageDomainService.MarkAsRead(request.MessageId);
        }
        await _unitOfWork.SaveAsync(cancellationToken);
        return Unit.Value;
    }
}
