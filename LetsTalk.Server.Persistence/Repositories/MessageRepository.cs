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
        await _context.Messages
            .Where(message => message.SenderId == recipientId && message.RecipientId == senderId && message.IsRead == false)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.IsRead, true).SetProperty(message => message.DateReadUnix, DateHelper.GetUnixTimestamp()));

        return await _context.Messages
            .Include(message => message.LinkPreview)
            .AsNoTracking()
            .Where(message => message.SenderId == senderId && message.RecipientId == recipientId || message.SenderId == recipientId && message.RecipientId == senderId)
            .OrderBy(mesage => mesage.DateCreatedUnix)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int messageId, int recipientId)
    {
        await _context.Messages
            .Where(message => message.Id == messageId && message.RecipientId == recipientId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.IsRead, true).SetProperty(message => message.DateReadUnix, DateHelper.GetUnixTimestamp()));
        await _context.SaveChangesAsync();
    }

    public async Task SetTextHtmlAsync(int messageId, string html)
    {
        await _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.TextHtml, html));
        await _context.SaveChangesAsync();
    }

    public async Task SetLinkPreviewAsync(int messageId, int linkPreviewId)
    {
        await _context.Messages
            .Where(message => message.Id == messageId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.LinkPreviewId, linkPreviewId));
        await _context.SaveChangesAsync();
    }
}
