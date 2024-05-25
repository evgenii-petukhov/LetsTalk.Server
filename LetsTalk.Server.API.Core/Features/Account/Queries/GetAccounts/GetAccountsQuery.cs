using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Account.Queries.GetAccounts;

public record GetAccountsQuery(string Id) : IRequest<List<AccountDto>>;
