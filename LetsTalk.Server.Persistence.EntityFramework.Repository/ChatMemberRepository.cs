using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ChatMemberRepository(LetsTalkDbContext context) : GenericRepository<Message>(context), IChatMemberRepository
{
    public Task<int> GetChatMemberIdAsync(int messageId, int accountId, CancellationToken cancellationToken = default)
    {
        var chatIds = _context.Messages.Where(x => x.Id == messageId).Select(x => x.ChatId);

        return _context.ChatMembers
            .Where(x => x.AccountId == accountId && chatIds.Contains(x.ChatId))
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int[]> GetChatMemberAccountIdsAsync(int chatId, CancellationToken cancellationToken = default)
    {
        return _context.ChatMembers
            .Where(x => x.ChatId == chatId)
            .Select(x => x.AccountId)
            .ToArrayAsync(cancellationToken);
    }
}
