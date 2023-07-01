using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public record GetMessagesQuery(int SenderId, int RecipientId, int PageIndex, int MessagesPerPage) : IRequest<List<MessageDto>>;
