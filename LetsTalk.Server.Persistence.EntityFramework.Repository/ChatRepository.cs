using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ChatRepository(LetsTalkDbContext context) : GenericRepository<Chat>(context), IChatRepository
{
    public async Task<List<ChatServiceModel>> GetChatsAsync(int accountId, CancellationToken cancellationToken = default)
    {
        var chats = await Context.ChatMembers
            .Where(cm => cm.AccountId == accountId)
            .Include(cm => cm.Chat!.ChatMembers!)
                .ThenInclude(cm => cm.Account)
            .Include(cm => cm.Chat!.Messages)
            .Select(cm => cm.Chat)
            .ToListAsync(cancellationToken);


        var chatMetrics = await Context.Messages
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
            .SelectMany(x => x.Statuses.DefaultIfEmpty(),
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
                LastMessageDate = g.Max(x => x.Message.DateCreatedUnix),
                LastMessageId = g.Max(x => x.Message.Id),
                UnreadCount = g.Count(x => x.Message.Id > x.ReadMessageId && x.Message.SenderId != accountId)
            })
            .ToListAsync(cancellationToken);

        return chats.Select(chat =>
        {
            var metrics = chatMetrics.FirstOrDefault(m => m.ChatId == chat!.Id);
            var otherAccount = chat!.ChatMembers!.FirstOrDefault(cm => cm.AccountId != accountId)?.Account;

            return new ChatServiceModel
            {
                Id = chat.Id.ToString(CultureInfo.InvariantCulture),
                ChatName = chat.IsIndividual && otherAccount != null ? $"{otherAccount.FirstName} {otherAccount.LastName}" : chat.Name,
                PhotoUrl = chat.IsIndividual ? otherAccount?.PhotoUrl : null,
                AccountTypeId = chat.IsIndividual ? otherAccount?.AccountTypeId : null,
                ImageId = chat.IsIndividual ? otherAccount?.ImageId : chat.ImageId,
                LastMessageDate = metrics?.LastMessageDate,
                LastMessageId = metrics?.LastMessageId.ToString(CultureInfo.InvariantCulture),
                UnreadCount = metrics?.UnreadCount ?? 0,
                IsIndividual = chat.IsIndividual,
                AccountIds = chat.ChatMembers!
                    .Where(cm => cm.AccountId != accountId)
                    .Select(cm => cm.AccountId.ToString(CultureInfo.InvariantCulture))
                    .ToArray()
            };
        }).ToList();
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
