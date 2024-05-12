using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IEmailLoginService
{
    Task<LoginResponseDto> LoginAsync(EmailLoginServiceModel model, CancellationToken cancellationToken);
}
