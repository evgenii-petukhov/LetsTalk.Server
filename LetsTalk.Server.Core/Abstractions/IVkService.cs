using LetsTalk.Server.Models.Authentication;

namespace LetsTalk.Server.Core.Abstractions;

public interface IVkService
{
    Task<LoginResponseDto> Login(LoginServiceInput model);
}
