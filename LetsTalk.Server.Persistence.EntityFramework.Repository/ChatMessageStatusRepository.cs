using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ChatMessageStatusRepository(LetsTalkDbContext context) : Repository(context), IChatMessageStatusRepository
{
    public async Task CreateAsync(ChatMessageStatus entity, CancellationToken cancellationToken = default)
    {
        await _context.ChatMessageStatuses.AddAsync(entity, cancellationToken);
    }
}
