using LetsTalk.Server.Models.Dtos;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Account.Queries.GetAccounts;

public record GetAccountsQuery(string Id) : IRequest<List<AccountDto>>;
