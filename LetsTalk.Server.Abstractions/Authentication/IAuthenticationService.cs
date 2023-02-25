using LetsTalk.Server.Models.Authentication;

namespace LetsTalk.Server.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<LoginServiceResult> Login(LoginServiceInput model);
}
