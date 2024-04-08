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

    public Task<int> GetChatMemberIdAsync(int messageId, int accountId)
    {
        var chatIds = _context.Messages.Where(x => x.Id == messageId).Select(x => x.ChatId);

        return _context.ChatMembers
            .Where(x => x.AccountId == accountId && chatIds.Contains(x.ChatId))
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
    }
}
