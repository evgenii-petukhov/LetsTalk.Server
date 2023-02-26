using LetsTalk.Server.Models.Authentication;

namespace LetsTalk.Server.Abstractions.Authentication;

public interface IVkService
{
    Task<LoginResponseDto> Login(LoginServiceInput model);
}
