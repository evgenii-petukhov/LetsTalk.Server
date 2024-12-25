using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record GenerateLoginCodeCommand(string Email) : IRequest<GenerateLoginCodeResponseDto>;