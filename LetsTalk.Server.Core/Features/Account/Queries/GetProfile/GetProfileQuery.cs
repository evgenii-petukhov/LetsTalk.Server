using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetProfile;

public record GetProfileQuery(string Id) : IRequest<ProfileDto>;
