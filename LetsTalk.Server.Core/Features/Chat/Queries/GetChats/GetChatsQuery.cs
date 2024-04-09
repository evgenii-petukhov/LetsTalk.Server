using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Chat.Queries.GetChats;

public record GetChatsQuery(string Id) : IRequest<List<ChatDto>>;
