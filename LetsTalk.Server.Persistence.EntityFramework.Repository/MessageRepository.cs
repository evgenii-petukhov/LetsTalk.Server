using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class MessageRepository(LetsTalkDbContext context) : GenericRepository<Message>(context), IMessageRepository
{
    public Task<List<Message>> GetPagedAsync(int senderId, int chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Include(message => message.LinkPreview)
            .Include(message => message.ImagePreview)
            .Where(message => message.ChatId == chatId)
            .OrderByDescending(mesage => mesage.DateCreatedUnix)
            .Skip(messagesPerPage * pageIndex)
            .Take(messagesPerPage)
            .OrderBy(mesage => mesage.DateCreatedUnix)
            .ToListAsync(cancellationToken);
    }
}
