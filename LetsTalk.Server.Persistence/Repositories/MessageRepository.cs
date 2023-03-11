using LetsTalk.Server.Abstractions.Repositories;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
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
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.IsRead, true).SetProperty(message => message.DateRead, DateTime.UtcNow));

        return await _context.Messages
            .Where(message => message.SenderId == senderId && message.RecipientId == recipientId || message.SenderId == recipientId && message.RecipientId == senderId)
            .OrderBy(mesage => mesage.DateCreated)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int messageId, int recipientId)
    {
        await _context.Messages
            .Where(message => message.Id == messageId && message.RecipientId == recipientId)
            .ExecuteUpdateAsync(x => x.SetProperty(message => message.IsRead, true).SetProperty(message => message.DateRead, DateTime.UtcNow));
        await _context.SaveChangesAsync();
    }
}
