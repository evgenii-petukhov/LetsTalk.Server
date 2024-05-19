using KafkaFlow;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.EmailService;

public class SendLoginCodeRequestHandler : IMessageHandler<SendLoginCodeRequest>
{
    public Task Handle(IMessageContext context, SendLoginCodeRequest request)
    {
        return Task.CompletedTask;
    }
}
