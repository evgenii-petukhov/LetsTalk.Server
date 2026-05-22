using LetsTalk.Server.Models.Dtos;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public class EmailLoginCommand : IRequest<LoginResponseDto>
{
    public string? Email { get; set; }

    public int Code { get; set; }
}
