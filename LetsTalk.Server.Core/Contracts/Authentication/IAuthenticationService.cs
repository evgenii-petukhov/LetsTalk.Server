using LetsTalk.Server.Core.Features.Authentication.Commands;
using LetsTalk.Server.Models.Authentication;

namespace LetsTalk.Server.Core.Contracts.Authentication;

public interface IAuthenticationService
{
    Task<LoginResponse> Login(LoginCommand model);
}
