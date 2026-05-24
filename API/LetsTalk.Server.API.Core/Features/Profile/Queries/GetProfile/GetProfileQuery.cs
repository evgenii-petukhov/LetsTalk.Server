using LetsTalk.Server.Models.Dtos;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Profile.Queries.GetProfile;

public record GetProfileQuery(string Id) : IRequest<ProfileDto>;
