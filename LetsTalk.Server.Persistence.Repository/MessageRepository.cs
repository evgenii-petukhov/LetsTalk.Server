using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Helpers;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repository;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Message>> GetAsync(int senderId, int recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(message => message.LinkPreview)
            .Include(message => message.ImagePreview)
            .Where(message => (message.SenderId == senderId && message.RecipientId == recipientId) || (message.SenderId == recipientId && message.RecipientId == senderId))
            .OrderByDescending(mesage => mesage.DateCreatedUnix)
            .Skip(messagesPerPage * pageIndex)
            .Take(messagesPerPage)
            .OrderBy(mesage => mesage.DateCreatedUnix)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public Task MarkAsReadAsync(int messageId, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(message => message.IsRead, true)
                .SetProperty(message => message.DateReadUnix, DateHelper.GetUnixTimestamp()), cancellationToken: cancellationToken);
    }

    public Task MarkAllAsReadAsync(int senderId, int recipientId, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(message => message.SenderId == recipientId && message.RecipientId == senderId && !message.IsRead)
            .ExecuteUpdateAsync(x => x
                .SetProperty(message => message.IsRead, true)
                .SetProperty(message => message.DateReadUnix, DateHelper.GetUnixTimestamp()), cancellationToken: cancellationToken);
    }

    public async Task SetTextHtmlAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        foreach (var message in messages)
        {
            await SetTextHtmlAsync(message.Id, message.TextHtml!, cancellationToken: cancellationToken);
        }
        await transaction.CommitAsync(cancellationToken);
    }

    public Task SetLinkPreviewAsync(int messageId, int linkPreviewId, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(message => message.LinkPreviewId, linkPreviewId), cancellationToken: cancellationToken);
    }

    public Task SetImagePreviewAsync(int messageId, int imageId, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(message => message.ImagePreviewId, imageId), cancellationToken: cancellationToken);
    }

    private Task SetTextHtmlAsync(int messageId, string html, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(message => message.TextHtml, html), cancellationToken: cancellationToken);
    }
}
