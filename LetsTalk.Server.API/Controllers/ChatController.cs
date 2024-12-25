using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.API.Core.Features.Chat.Queries.GetChats;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LetsTalk.Server.API.Validation;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.API.Core.Commands;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class ChatController(
    IMediator mediator) : ApiController
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<ChatDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetChatsQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ChatDtoBase>> PostAsync(CreateIndividualChatRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateIndividualChatRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var cmd = new CreateIndividualChatCommand(GetAccountId(), request.AccountId!);
        var response = await _mediator.Send(cmd, cancellationToken);
        return Ok(response.Dto);
    }
}
