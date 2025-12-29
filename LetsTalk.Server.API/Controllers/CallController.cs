using LetsTalk.Server.API.Models.Chat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.VideoCall.Queries.GetCallSettings;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class CallController(
    IMediator mediator) : ApiController
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("CallSettings")]
    public async Task<ActionResult<CallSettingsDto>> GetCallSettingsAsync(CancellationToken cancellationToken)
    {
        var query = new GetCallSettingsQuery();
        var settings = await _mediator.Send(query, cancellationToken);
        return Ok(settings);
    }

    [HttpPost("StartOutgoingCall")]
    public async Task<ActionResult> StartOutgoingCallAsync(StartOutgoingCallRequest request, CancellationToken cancellationToken)
    {
        var cmd = new StartOutgoingCallCommand(GetAccountId(), request.AccountId!, request.Offer!);
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }

    [HttpPost("HandleIncomingCall")]
    public async Task<ActionResult> HandleIncomingCallAsync(HandleIncomingCallRequest request, CancellationToken cancellationToken)
    {
        var cmd = new HandleIncomingCallCommand(request.AccountId!, request.Answer!);
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }
}
