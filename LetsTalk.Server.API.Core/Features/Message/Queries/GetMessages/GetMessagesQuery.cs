using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Message.Queries.GetMessages;

public record GetMessagesQuery(string SenderId, string ChatId, int PageIndex, int MessagesPerPage) : IRequest<List<MessageDto>>;
