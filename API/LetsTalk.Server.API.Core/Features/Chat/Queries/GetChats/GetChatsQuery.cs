using LetsTalk.Server.Models.Dtos;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Chat.Queries.GetChats;

public record GetChatsQuery(string Id) : IRequest<List<ChatDto>>;
