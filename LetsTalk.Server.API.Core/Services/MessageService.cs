using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.API.Core.Services;

public class MessageService(
    IMessageAgnosticService messageAgnosticService) : IMessageService
{
    private readonly IMessageAgnosticService _messageAgnosticService = messageAgnosticService;

    public async Task<IReadOnlyList<MessageServiceModel>> GetPagedAsync(string chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken)
    {
        return await _messageAgnosticService.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken);
    }
}
