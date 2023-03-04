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
        return await _context.Messages
            .Where(message => message.SenderId == senderId && message.RecipientId == recipientId || message.SenderId == recipientId && message.RecipientId == senderId)
            .OrderBy(mesage => mesage.DateCreated)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int messageId, int recipientId)
    {
        await _context.Messages
            .Where(message => message.Id == messageId && message.RecipientId == recipientId)
            .ExecuteUpdateAsync(message => message.SetProperty(x => x.IsRead, true).SetProperty(x => x.DateRead, DateTime.Now));
        await _context.SaveChangesAsync();
    }
}
