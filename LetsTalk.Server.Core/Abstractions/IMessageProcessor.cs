using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageProcessor
{
    MessageDto GetMessageDto(Message message, int senderId);

    void SetTextHtml(Message message);
}
