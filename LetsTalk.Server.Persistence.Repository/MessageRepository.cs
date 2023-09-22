using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repository;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Message>> GetPagedAsTrackingAsync(int senderId, int recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .AsTracking()
            .Include(message => message.LinkPreview)
            .Include(message => message.ImagePreview)
            .Where(message => (message.SenderId == senderId && message.RecipientId == recipientId) || (message.SenderId == recipientId && message.RecipientId == senderId))
            .OrderByDescending(mesage => mesage.DateCreatedUnix)
            .Skip(messagesPerPage * pageIndex)
            .Take(messagesPerPage)
            .OrderBy(mesage => mesage.DateCreatedUnix)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
