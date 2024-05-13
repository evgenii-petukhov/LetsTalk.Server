using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Authentication.Commands.EmailLogin;

public record GenerateLoginCodeCommand(string Email) : IRequest<GenerateLoginCodeResponseDto>;