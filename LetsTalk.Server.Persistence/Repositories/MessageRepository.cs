using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Message>> GetAsync(int senderId, int recipientId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(message => message.LinkPreview)
            .Where(message => (message.SenderId == senderId && message.RecipientId == recipientId) || (message.SenderId == recipientId && message.RecipientId == senderId))
            .OrderBy(mesage => mesage.DateCreatedUnix)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public Task MarkAsReadAsync(int messageId, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.IsRead, true).SetProperty(message => message.DateReadUnix, DateHelper.GetUnixTimestamp()), cancellationToken: cancellationToken);
    }

    public Task MarkAllAsReadAsync(int senderId, int recipientId, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(message => message.SenderId == recipientId && message.RecipientId == senderId && !message.IsRead)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.IsRead, true).SetProperty(message => message.DateReadUnix, DateHelper.GetUnixTimestamp()), cancellationToken: cancellationToken);
    }

    public Task SetTextHtmlAsync(int messageId, string html, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.TextHtml, html), cancellationToken: cancellationToken);
    }

    public Task SetLinkPreviewAsync(int messageId, int linkPreviewId, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.LinkPreviewId, linkPreviewId), cancellationToken: cancellationToken);
    }
}
