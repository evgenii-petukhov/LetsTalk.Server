using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;

public record GenerateLoginCodeCommand(string Email) : IRequest<GenerateLoginCodeResponseDto>;