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
    private readonly IMessageDomainService _messageDomainService;

    public LinkPreviewRequestHandler(
        ILinkPreviewGenerator linkPreviewGenerator,
        IUnitOfWork unitOfWork,
        IMessageDomainService messageDomainService)
    {
        _linkPreviewGenerator = linkPreviewGenerator;
        _unitOfWork = unitOfWork;
        _messageDomainService = messageDomainService;
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

        await _messageDomainService.SetLinkPreviewAsync(linkPreview, request.MessageId);
        await _unitOfWork.SaveAsync();
    }
}
