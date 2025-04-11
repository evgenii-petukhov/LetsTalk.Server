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

    public Task<Dictionary<int, ChatMetric>> GetChatMetricsAsync(int accountId, CancellationToken cancellationToken = default)
    {
        return Context.Chats
            .Where(m => m.ChatMembers!.Any(cm => cm.AccountId == accountId))
            .SelectMany(m => m.Messages!)
            .Select(m => new
            {
                m.ChatId,
                m.Id,
                m.DateCreatedUnix,
                ReadMessageId = Context.ChatMessageStatuses
                    .Where(s => s.AccountId == accountId && s.MessageId == m.Id)
                    .Select(s => s.MessageId)
                    .FirstOrDefault()
            })
            .GroupBy(m => m.ChatId)
            .Select(g => new
            {
                ChatId = g.Key,
                LastMessageId = g.Max(m => m.Id),
                LastMessageDate = g.Max(m => m.DateCreatedUnix),
                LastReadMessageId = g.Max(m => m.ReadMessageId)
            })
            .Join(Context.Messages.Where(m => m.SenderId != accountId),
                metric => metric.ChatId,
                message => message.ChatId,
                (metric, message) => new { metric, message })
            .GroupBy(x => x.metric)
            .Select(g => new ChatMetric
            {
                ChatId = g.Key.ChatId,
                LastMessageId = g.Key.LastMessageId,
                LastMessageDate = g.Key.LastMessageDate,
                UnreadCount = g.Count(x => x.message.Id > g.Key.LastReadMessageId)
            })
            .ToDictionaryAsync(x => x.ChatId, cancellationToken);
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
