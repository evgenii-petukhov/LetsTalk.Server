using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ChatRepository(LetsTalkDbContext context) : GenericRepository<Chat>(context), IChatRepository
{
    public async Task<List<ChatServiceModel>> GetChatsAsync(int accountId, CancellationToken cancellationToken = default)
    {
        var chats = await _context.ChatMembers
            .Include(cm => cm.Chat)
            .Include(cm => cm.Account)
            .Where(cm => cm.AccountId != accountId && _context.ChatMembers.Where(x => x.AccountId == accountId).Select(x => x.ChatId).Contains(cm.ChatId))
            .GroupBy(x => x.Chat)
            .Select(g => new
            {
                Chat = g.Key,
                Accounts = g.Select(x => x.Account).ToList()
            })
            .Select(g => new
            {
                g.Chat,
                g.Accounts
            })
            .ToListAsync(cancellationToken);

        var metrics = await _context.ChatMembers
            .Where(cm => cm.AccountId == accountId)
            .GroupJoin(_context.Messages, x => x.ChatId, x => x.ChatId, (x, y) => new
            {
                x.ChatId,
                Messages = y
            })
            .SelectMany(x => x.Messages.DefaultIfEmpty(), (x, y) => new
            {
                x.ChatId,
                Message = y
            })
            .GroupJoin(
                _context.ChatMessageStatuses,
                x => x.Message!.Id,
                x => x.MessageId,
                (x, y) => new
                {
                    x.ChatId,
                    x.Message,
                    Statuses = y
                })
            .SelectMany(x => x.Statuses.DefaultIfEmpty(), (x, y) => new
            {
                x.ChatId,
                x.Message,
                ReadMessageId = y == null ? 0 : y.MessageId
            })
            .GroupBy(x => x.ChatId)
            .Select(g => new
            {
                ChatId = g.Key,
                LastMessageDate = g.Max(x => x.Message!.DateCreatedUnix),
                LastMessageId = g.Max(x => x.Message!.Id),
                LastReadMessageId = g.Max(x => x.ReadMessageId)
            })
            .GroupJoin(_context.Messages, x => x.ChatId, x => x.ChatId, (x, y) => new
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
            .Select(g => new
            {
                g.Key.ChatId,
                g.Key.LastMessageId,
                g.Key.LastMessageDate,
                UnreadCount = g.Count(x => x.Message!.Id > x.LastReadMessageId && x.Message.SenderId != accountId)
            })
            .ToListAsync(cancellationToken);

        return chats
            .Join(metrics, x => x.Chat!.Id, x => x.ChatId, (x, y) => new
            {
                x.Chat,
                AccountIds = x.Accounts.ConvertAll(x => x!.Id)
                    .Where(x => x != accountId)
                    .Select(x => x.ToString(CultureInfo.InvariantCulture))
                    .ToArray(),
                Account = x.Accounts.FirstOrDefault(),
                Metrics = y
            })
            .Select(g => new ChatServiceModel
            {
                Id = g.Chat!.Id.ToString(CultureInfo.InvariantCulture),
                ChatName = g.Chat!.IsIndividual ? $"{g.Account!.FirstName} {g.Account.LastName}" : g.Chat.Name,
                PhotoUrl = g.Chat.IsIndividual ? g.Account!.PhotoUrl : null,
                AccountTypeId = g.Chat.IsIndividual ? g.Account!.AccountTypeId : null,
                ImageId = g.Chat.IsIndividual ? g.Account!.ImageId : g.Chat.ImageId,
                LastMessageDate = g.Metrics.LastMessageDate,
                LastMessageId = g.Metrics.LastMessageId.ToString(CultureInfo.InvariantCulture),
                UnreadCount = g.Metrics.UnreadCount,
                IsIndividual = g.Chat!.IsIndividual,
                AccountIds = g.AccountIds
            })
            .ToList();
    }

    public Task<bool> IsChatIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Chats
            .AnyAsync(chat => chat.Id == id, cancellationToken);
    }

    public Task<Chat> GetIndividualChatByAccountIdsAsync(int[] accountIds, CancellationToken cancellationToken = default)
    {
        return _context.ChatMembers
            .GroupBy(x => x.Chat)
            .Where(g => g.Key!.IsIndividual && g.All(x => accountIds.Contains(x.AccountId)))
            .Select(g => g.Key)
            .FirstOrDefaultAsync(cancellationToken)!;
    }
}
