using MediatR;
using Microsoft.AspNetCore.Mvc;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.VideoCall.Queries.GetCallSettings;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.API.Models.Call;

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
    public async Task<ActionResult<StartOutgoingCallDto>> StartOutgoingCallAsync(StartOutgoingCallRequest request, CancellationToken cancellationToken)
    {
        var cmd = new StartOutgoingCallCommand(
            GetAccountId(),
            request.ChatId!,
            request.Offer!,
            request.IceGatheringElapsedMs,
            request.IceGatheringCollectedAll,
            request.ConnectionDiagnostics!);
        var dto = await _mediator.Send(cmd, cancellationToken);
        return Ok(dto);
    }

    [HttpPost("HandleIncomingCall")]
    public async Task<ActionResult> HandleIncomingCallAsync(HandleIncomingCallRequest request, CancellationToken cancellationToken)
    {
        var cmd = new HandleIncomingCallCommand(
            request.CallId!,
            GetAccountId(),
            request.ChatId!,
            request.Answer!,
            request.IceGatheringElapsedMs,
            request.IceGatheringCollectedAll,
            request.ConnectionDiagnostics!);
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }

    [HttpPost("LogConnectionEstablished")]
    public async Task<ActionResult> LogConnectionEstablishedAsync(LogConnectionEstablishedRequest request, CancellationToken cancellationToken)
    {
        var cmd = new LogConnectionEstablishedCommand(
            request.CallId!,
            GetAccountId(),
            request.ChatId!,
            request.ConnectionDiagnostics!);
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }

    [HttpPost("LogConnectionFailed")]
    public async Task<ActionResult> LogConnectionFailedAsync(LogConnectionFailedRequest request, CancellationToken cancellationToken)
    {
        var cmd = new LogConnectionFailedCommand(
            request.CallId!,
            GetAccountId(),
            request.ChatId!,
            request.ConnectionDiagnostics!,
            request.Error!,
            request.StackTrace!);
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }
}
