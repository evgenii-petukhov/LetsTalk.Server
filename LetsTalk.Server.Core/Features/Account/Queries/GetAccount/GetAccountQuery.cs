using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetAccount;

public record GetAccountQuery(int Id): IRequest<AccountDto>;
