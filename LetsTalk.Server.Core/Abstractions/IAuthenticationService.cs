using LetsTalk.Server.API.Models;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IAuthenticationService
{
    Task<LoginResponseDto> Login(LoginServiceInput model);
}