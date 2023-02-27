using LetsTalk.Server.Models.Message;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public record CreateMessageCommand(int SenderId, int RecipientId, string Text): IRequest<MessageDto>;