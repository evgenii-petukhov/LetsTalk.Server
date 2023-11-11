using LetsTalk.Server.Persistence.DatabaseAgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.DatabaseAgnosticServices.Abstractions;

public interface IMessageDatabaseAgnosticService
{
    Task<MessageAgnosticModel> CreateMessageAsync(
        int senderId,
        int recipientId,
        string? text,
        string? textHtml,
        int? imageId,
        CancellationToken cancellationToken);
}
