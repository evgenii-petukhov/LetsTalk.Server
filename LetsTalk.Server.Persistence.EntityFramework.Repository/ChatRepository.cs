using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ChatRepository(LetsTalkDbContext context) : GenericRepository<Chat>(context), IChatRepository
{
    public Task<List<Chat>> GetChatsByAccountIdAsync(int accountId, CancellationToken cancellationToken = default)
    {
        return Context.ChatMembers
            .Where(cm => cm.AccountId == accountId)
            .Include(cm => cm.Chat!.ChatMembers!)
                .ThenInclude(cm => cm.Account)
                .ThenInclude(a => a!.Image)
            .Select(cm => cm.Chat!)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ChatMetric>> GetChatMetricsAsync(int accountId, CancellationToken cancellationToken = default)
    {
        return Context.Messages
            .Where(m => m.Chat!.ChatMembers!.Any(cm => cm.AccountId == accountId))
            .GroupJoin(Context.ChatMessageStatuses,
                m => m.Id,
                cms => cms.MessageId,
                (m, statuses) => new
                {
                    m.ChatId,
                    Message = m,
                    Statuses = statuses
                })
            .SelectMany(x => x.Statuses.Where(s => s.AccountId == accountId).DefaultIfEmpty(),
                (x, status) => new
                {
                    x.ChatId,
                    x.Message,
                    ReadMessageId = status == null ? 0 : status.MessageId
                })
            .GroupBy(x => x.ChatId)
            .Select(g => new
            {
                ChatId = g.Key,
                LastMessageDate = g.Max(x => x.Message!.DateCreatedUnix),
                LastMessageId = g.Max(x => x.Message!.Id),
                LastReadMessageId = g.Max(x => x.ReadMessageId)
            })
            .GroupJoin(Context.Messages, x => x.ChatId, x => x.ChatId, (x, y) => new
            {
                x.ChatId,
                x.LastMessageId,
                x.LastReadMessageId,
                x.LastMessageDate,
                Messages = y
            })
            .SelectMany(x => x.Messages.DefaultIfEmpty(), (x, y) => new
            {
                x.ChatId,
                x.LastMessageId,
                x.LastReadMessageId,
                x.LastMessageDate,
                Message = y
            })
            .GroupBy(x => new
            {
                x.ChatId,
                x.LastMessageId,
                x.LastReadMessageId,
                x.LastMessageDate,
            })
            .Select(g => new ChatMetric
            {
                ChatId = g.Key.ChatId,
                LastMessageId = g.Key.LastMessageId,
                LastMessageDate = g.Key.LastMessageDate,
                UnreadCount = g.Count(x => x.Message!.Id > x.LastReadMessageId && x.Message.SenderId != accountId)
            })
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsChatIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return Context.Chats
            .AnyAsync(chat => chat.Id == id, cancellationToken);
    }

    public Task<Chat> GetIndividualChatByAccountIdsAsync(int[] accountIds, CancellationToken cancellationToken = default)
    {
        return Context.ChatMembers
            .GroupBy(x => x.Chat)
            .Where(g => g.Key!.IsIndividual && g.All(x => accountIds.Contains(x.AccountId)))
            .Select(g => g.Key)
            .FirstOrDefaultAsync(cancellationToken)!;
    }
}
