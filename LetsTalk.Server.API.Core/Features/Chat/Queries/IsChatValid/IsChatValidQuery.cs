using MediatR;

namespace LetsTalk.Server.API.Core.Features.Chat.Queries.IsChatValid;

public record IsChatValidQuery(string Id) : IRequest<bool>;
