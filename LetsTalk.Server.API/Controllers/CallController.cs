using LetsTalk.Server.API.Models.Chat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LetsTalk.Server.API.Core.Commands;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class CallController(
    IMediator mediator) : ApiController
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("Initialize")]
    public async Task<ActionResult> InitializeAsync(InitializeCallRequest request, CancellationToken cancellationToken)
    {
        var cmd = new InitializeCallCommand(request.AccountId!, request.SessionDescription!);
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }

    [HttpPost("Accept")]
    public async Task<ActionResult> AcceptAsync(InitializeCallRequest request, CancellationToken cancellationToken)
    {
        var cmd = new AcceptCallCommand(request.AccountId!, request.SessionDescription!);
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }
}
