using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;

public class EmailLoginCommand : MediatR.IRequest<LoginResponseDto>
{
    public string? Email { get; set; }

    public int Code { get; set; }
}
