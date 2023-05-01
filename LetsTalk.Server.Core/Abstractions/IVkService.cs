using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IVkService
{
    Task<LoginResponseDto> Login(LoginServiceInput model);
}
