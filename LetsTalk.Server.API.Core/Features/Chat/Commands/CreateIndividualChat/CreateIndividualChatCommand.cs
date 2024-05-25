using LetsTalk.Server.API.Models.Chat;
using MediatR;

namespace LetsTalk.Server.Core.Features.Chat.Commands.CreateIndividualChat;

public record CreateIndividualChatCommand(string InvitingAccountId, string AccountId) : IRequest<CreateIndividualChatResponse>;
