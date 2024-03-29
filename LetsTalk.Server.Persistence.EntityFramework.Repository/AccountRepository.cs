using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class AccountRepository(LetsTalkDbContext context) : GenericRepository<Account>(context), IAccountRepository
{
    public override Task<Account> GetByIdAsTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .Include(x => x.Image)
            .AsTracking()
            .FirstOrDefaultAsync(account => account.Id == id, cancellationToken)!;
    }

    public Task<Account> GetByExternalIdAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .FirstOrDefaultAsync(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountType, cancellationToken)!;
    }

    public Task<Account> GetByExternalIdAsTrackingAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .AsTracking()
            .FirstOrDefaultAsync(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountType, cancellationToken)!;
    }

    public async Task<List<ContactServiceModel>> GetContactsAsync(int id, CancellationToken cancellationToken = default)
    {
        var chats = await _context.ChatMembers
            .Include(cm => cm.Chat)
            .Include(cm => cm.Account)
            .Where(cm => cm.AccountId != id && _context.ChatMembers.Where(x => x.AccountId == id).Select(x => x.ChatId).Contains(cm.ChatId))
            .GroupBy(x => x.Chat)
            .Select(g => new
            {
                Chat = g.Key,
                Accounts = g.Select(x => x.Account!).ToList()
            })
            .ToListAsync(cancellationToken);

        var metrics = await _context.ChatMembers
            .Where(cm => cm.AccountId == id)
            .GroupJoin(_context.Messages, x => x.ChatId, x => x.ChatId, (x, y) => new
            {
                ChatMember = x,
                Messages = y
            })
            .SelectMany(x => x.Messages.DefaultIfEmpty(), (x, y) => new
            {
                x.ChatMember,
                Message = y
            })
            .GroupJoin(
                _context.ChatMessageStatuses,
                x => new
                {
                    MessageId = x.Message!.Id,
                    ChatMemberId = x.ChatMember.Id,
                    IsRead = true
                },
                x => new
                {
                    x.MessageId,
                    x.ChatMemberId,
                    x.IsRead
                },
                (x, y) => new
                {
                    x.ChatMember,
                    x.Message,
                    Statuses = y
                })
            .SelectMany(x => x.Statuses.DefaultIfEmpty(), (x, y) => new
            {
                x.ChatMember,
                x.Message,
                y!.IsRead,
                IsMine = x.Message!.SenderId == id,
                y.DateReadUnix
            })
            .GroupBy(x => x.ChatMember.ChatId)
            .Select(g => new
            {
                ChatId = g.Key,
                LastMessageDate = g.Max(x => x.DateReadUnix),
                LastMessageId = g.Max(x => x.Message!.Id),
                UnreadCount = g.Count() - g.Count(x => x.IsMine) - g.Count(x => x.IsRead)
            })
            .ToListAsync(cancellationToken);

        return chats
            .Join(metrics, x => x.Chat!.Id, x => x.ChatId, (x, y) => new
            {
                x.Chat,
                x.Accounts,
                Metrics = y
            })
            .Select(g => new ContactServiceModel
            {
                Id = g.Chat!.Id.ToString(),
                ChatName = GetChatName(g.Chat, g.Accounts),
                PhotoUrl = g.Chat.IsIndividual ? g.Accounts.FirstOrDefault()?.PhotoUrl : null,
                AccountTypeId = g.Accounts.FirstOrDefault()?.AccountTypeId,
                LastMessageDate = g.Metrics.LastMessageDate,
                LastMessageId = g.Metrics.LastMessageId.ToString(),
                UnreadCount = g.Metrics.UnreadCount,
                ImageId = g.Chat.IsIndividual ? g.Accounts.FirstOrDefault()?.ImageId : g.Chat.ImageId
            })
            .ToList();
    }

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .AnyAsync(account => account.Id == id, cancellationToken);
    }

    private static string? GetChatName(Chat chat, List<Account>? accounts)
    {
        var account = accounts?.FirstOrDefault();

        if (account == null && (chat.IsIndividual || string.IsNullOrEmpty(chat.Name)))
        {
            return null;
        }

        if (chat.IsIndividual)
        {
            return $"{account!.FirstName} {account.LastName}";
        }
        else
        {
            return string.IsNullOrEmpty(chat.Name)
                ? string.Join(',', accounts!.Select(a => $"{a!.FirstName} {a.LastName}"))
                : chat.Name;
        }
    }
}
