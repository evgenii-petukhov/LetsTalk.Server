using LetsTalk.Server.Models.Authentication;

namespace LetsTalk.Server.Core.Abstractions;

public interface IAuthenticationService
{
    Task<LoginResponseDto> Login(LoginServiceInput model);
}