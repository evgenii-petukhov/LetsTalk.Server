using LetsTalk.Server.Models.Authentication;

namespace LetsTalk.Server.Core.Abstractions;

public interface IFacebookService
{
    Task<LoginResponseDto> Login(LoginServiceInput model);
}
