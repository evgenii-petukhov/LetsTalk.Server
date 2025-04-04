﻿using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;
using LetsTalk.Server.API.Core.Commands;

namespace LetsTalk.Server.API.Core.Features.Chat.Commands.CreateIndividualChat;

public class CreateIndividualChatCommandHandler(
    IChatAgnosticService chatAgnosticService,
    IAccountAgnosticService accountAgnosticService,
    IChatCacheManager chatCacheManager) : IRequestHandler<CreateIndividualChatCommand, CreateIndividualChatResponse>
{
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;
    private readonly IChatCacheManager _chatCacheManager = chatCacheManager;

    public async Task<CreateIndividualChatResponse> Handle(CreateIndividualChatCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateIndividualChatCommandValidator(_accountAgnosticService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var chatId = await _chatAgnosticService.CreateIndividualChatAsync([request.InvitingAccountId, request.AccountId], cancellationToken);

        await _chatCacheManager.ClearAsync(request.InvitingAccountId);
        await _chatCacheManager.ClearAsync(request.AccountId);

        return new CreateIndividualChatResponse
        {
            Dto = new ChatDtoBase
            {
                Id = chatId
            }
        };
    }
}
