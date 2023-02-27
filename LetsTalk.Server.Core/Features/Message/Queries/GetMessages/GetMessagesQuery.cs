using LetsTalk.Server.Models.Message;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public record GetMessagesQuery(int SenderId, int RecipientId): IRequest<List<MessageDto>>;
