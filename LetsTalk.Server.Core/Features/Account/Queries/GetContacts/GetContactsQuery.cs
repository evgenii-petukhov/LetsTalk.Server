using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetContacts;

public record GetContactsQuery(int Id) : IRequest<List<AccountDto>>;
