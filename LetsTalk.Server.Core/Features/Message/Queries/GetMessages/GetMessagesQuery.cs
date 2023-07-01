using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public record GetMessagesQuery(int SenderId, int RecipientId, int pageIndex, int messagesPerPage) : IRequest<List<MessageDto>>;
