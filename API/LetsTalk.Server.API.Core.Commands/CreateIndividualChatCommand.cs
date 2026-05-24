using LetsTalk.Server.API.Models.Chat;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record CreateIndividualChatCommand(string InvitingAccountId, string AccountId) : IRequest<CreateIndividualChatResponse>;
