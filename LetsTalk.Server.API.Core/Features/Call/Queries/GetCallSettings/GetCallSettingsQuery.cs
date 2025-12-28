using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Call.Queries.GetCallSettings;

public record GetCallSettingsQuery() : IRequest<CallSettingsDto>;
