using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ChatRepository(LetsTalkDbContext context) : GenericRepository<Account>(context), IChatRepository
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
                Accounts = g.Select(x => x.Account!).ToList()
            })
            .Select(x => new
            {
                x.Chat,
                x.Accounts,
                FirstAccount = x.Accounts.FirstOrDefault()
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
                ReadMessageId = y!.MessageId
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
                x.Accounts,
                x.FirstAccount,
                Metrics = y
            })
            .Select(g => new ChatServiceModel
            {
                Id = g.Chat!.Id.ToString(),
                ChatName = GetChatName(g.Chat, g.Accounts),
                PhotoUrl = g.Chat.IsIndividual ? g.FirstAccount?.PhotoUrl : null,
                AccountTypeId = g.Chat.IsIndividual ? g.FirstAccount?.AccountTypeId : null,
                ImageId = g.Chat.IsIndividual ? g.FirstAccount?.ImageId : g.Chat.ImageId,
                LastMessageDate = g.Metrics.LastMessageDate,
                LastMessageId = g.Metrics.LastMessageId.ToString(),
                UnreadCount = g.Metrics.UnreadCount
            })
            .ToList();
    }

    private static string? GetChatName(Chat chat, List<Account>? accounts)
    {
        if ((accounts == null || accounts.Count == 0) && (chat.IsIndividual || string.IsNullOrEmpty(chat.Name)))
        {
            return null;
        }

        if (chat.IsIndividual)
        {
            return accounts?.Count > 0
                ? $"{accounts[0].FirstName} {accounts[0].LastName}"
                : null;
        }
        else
        {
            return string.IsNullOrEmpty(chat.Name)
                ? string.Join(',', accounts!.Select(a => $"{a!.FirstName} {a.LastName}"))
                : chat.Name;
        }
    }
}
