using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.ImageProcessor.Models;

namespace LetsTalk.Server.ImageProcessor;

public class ImageResizeRequestHandler : IMessageHandler<ImageResizeRequest>
{
    public Task Handle(IMessageContext context, ImageResizeRequest message)
    {
        throw new NotImplementedException();
    }
}
