using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageProcessor
{
    public Task<MessageDto> GetMessageDto(Domain.Message message, int senderId);
}
