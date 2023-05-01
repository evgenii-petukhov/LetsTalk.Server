using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Queries.GetProfile;

public record GetProfileQuery(int Id) : IRequest<AccountDto>;
