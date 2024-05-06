using LetsTalk.Server.API.Models.Chat;
using MediatR;

namespace LetsTalk.Server.Core.Features.Chat.Commands.CreateIndividualChatCommand;

public record CreateIndividualChatCommand(string InvitingAccountId, string AccountId) : IRequest<CreateIndividualChatResponse>;
