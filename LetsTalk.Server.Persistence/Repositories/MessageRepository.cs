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

    public async Task<IReadOnlyList<Message>> GetAsync(int senderId, int recipientId)
    {
        return await _context.Messages
            .Include(message => message.LinkPreview)
            .AsNoTracking()
            .Where(message => (message.SenderId == senderId && message.RecipientId == recipientId) || (message.SenderId == recipientId && message.RecipientId == senderId))
            .OrderBy(mesage => mesage.DateCreatedUnix)
            .ToListAsync();
    }

    public Task MarkAsReadAsync(int messageId)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.IsRead, true).SetProperty(message => message.DateReadUnix, DateHelper.GetUnixTimestamp()));
    }

    public Task MarkAllAsReadAsync(int senderId, int recipientId)
    {
        return _context.Messages
            .AsNoTracking()
            .Where(message => message.SenderId == recipientId && message.RecipientId == senderId && !message.IsRead)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.IsRead, true).SetProperty(message => message.DateReadUnix, DateHelper.GetUnixTimestamp()));
    }

    public Task SetTextHtmlAsync(int messageId, string html)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.TextHtml, html));
    }

    public Task SetLinkPreviewAsync(int messageId, int linkPreviewId)
    {
        return _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.LinkPreviewId, linkPreviewId));
    }
}
