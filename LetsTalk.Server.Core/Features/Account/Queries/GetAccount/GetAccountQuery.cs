using LetsTalk.Server.Models.Account;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetAccount;

public record GetAccountQuery(int Id): IRequest<AccountDto>;
