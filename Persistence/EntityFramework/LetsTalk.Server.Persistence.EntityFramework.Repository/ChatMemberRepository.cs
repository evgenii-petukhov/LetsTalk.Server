using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ChatMemberRepository(LetsTalkDbContext context)
    : GenericRepository<Message>(context), IChatMemberRepository
{
    public Task<List<int>> GetChatMemberAccountIdsAsync(int chatId, CancellationToken cancellationToken = default)
    {
        return Context.ChatMembers
            .Where(x => x.ChatId == chatId)
            .Select(x => x.AccountId)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsAccountChatMemberAsync(int chatId, int accountId, CancellationToken cancellationToken = default)
    {
        return Context.ChatMembers
            .Where(x => x.ChatId == chatId && accountId == x.AccountId)
            .AnyAsync(cancellationToken);
    }
}
