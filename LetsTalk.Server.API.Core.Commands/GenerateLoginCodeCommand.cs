using LetsTalk.Server.Models.Dtos;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public record GenerateLoginCodeCommand(string Email) : IRequest<GenerateLoginCodeResponseDto>;