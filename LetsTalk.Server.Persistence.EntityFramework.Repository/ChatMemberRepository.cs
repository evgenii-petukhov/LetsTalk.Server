using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ChatMemberRepository(LetsTalkDbContext context) : GenericRepository<Message>(context), IChatMemberRepository
{
    public Task<int[]> GetChatMemberAccountIdsAsync(int chatId, CancellationToken cancellationToken = default)
    {
        return _context.ChatMembers
            .Where(x => x.ChatId == chatId)
            .Select(x => x.AccountId)
            .ToArrayAsync(cancellationToken);
    }
}
