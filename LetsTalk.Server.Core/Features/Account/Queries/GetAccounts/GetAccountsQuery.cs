using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetAccounts;

public record GetAccountsQuery(int Id) : IRequest<List<AccountDto>>;
