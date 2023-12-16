using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public Task<List<Message>> GetPagedAsync(int senderId, int recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Include(message => message.LinkPreview)
            .Include(message => message.ImagePreview)
            .Where(message => (message.SenderId == senderId && message.RecipientId == recipientId) || (message.SenderId == recipientId && message.RecipientId == senderId))
            .OrderByDescending(mesage => mesage.DateCreatedUnix)
            .Skip(messagesPerPage * pageIndex)
            .Take(messagesPerPage)
            .OrderBy(mesage => mesage.DateCreatedUnix)
            .ToListAsync(cancellationToken);
    }

    public Task MarkAllAsReadAsync(int senderId, int recipientId, int messageId)
    {
        return _context.Messages
            .AsTracking()
            .Where(message => message.Id <= messageId && message.SenderId == senderId && message.RecipientId == recipientId && !message.IsRead)
            .ForEachAsync(message => message.MarkAsRead());
    }
}
