using LetsTalk.Server.Models.Authentication;

namespace LetsTalk.Server.Abstractions.Authentication;

public interface IFacebookService
{
    Task<LoginResponseDto> Login(LoginServiceInput model);
}
