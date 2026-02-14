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
        var cmd = new StartOutgoingCallCommand(
            GetAccountId(),
            request.ChatId!,
            request.Offer!,
            request.ConnectionState!,
            request.LocalCandidateTypes!,
            request.RemoteCandidateTypes!,
            request.Platform!,
            request.Browser!,
            request.IceGatheringElapsedMs,
            request.IceGatheringCollectedAll);
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }

    [HttpPost("HandleIncomingCall")]
    public async Task<ActionResult> HandleIncomingCallAsync(HandleIncomingCallRequest request, CancellationToken cancellationToken)
    {
        var cmd = new HandleIncomingCallCommand(
            request.CallId!,
            GetAccountId(),
            request.ChatId!,
            request.Answer!,
            request.ConnectionState!,
            request.LocalCandidateTypes!,
            request.RemoteCandidateTypes!,
            request.Platform!,
            request.Browser!,
            request.IceGatheringElapsedMs,
            request.IceGatheringCollectedAll);
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }
}
