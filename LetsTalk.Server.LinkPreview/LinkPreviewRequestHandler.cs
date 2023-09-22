using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly ILinkPreviewGenerator _linkPreviewGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageRepository _messageRepository;

    public LinkPreviewRequestHandler(
        ILinkPreviewGenerator linkPreviewGenerator,
        IUnitOfWork unitOfWork,
        IMessageRepository messageRepository)
    {
        _linkPreviewGenerator = linkPreviewGenerator;
        _unitOfWork = unitOfWork;
        _messageRepository = messageRepository;
    }

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (request.Url == null)
        {
            return;
        }

        var linkPreview = await _linkPreviewGenerator.GetLinkPreviewAsync(request.Url);

        if (linkPreview == null)
        {
            return;
        }

        var message = await _messageRepository.GetByIdAsTrackingAsync(request.MessageId);
        message.SetLinkPreview(linkPreview);
        await _unitOfWork.SaveAsync();
    }
}
