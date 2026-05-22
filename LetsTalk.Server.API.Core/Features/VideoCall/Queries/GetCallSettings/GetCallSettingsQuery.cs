using LetsTalk.Server.Models.Dtos;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.VideoCall.Queries.GetCallSettings;

public record GetCallSettingsQuery() : IRequest<CallSettingsDto>;
