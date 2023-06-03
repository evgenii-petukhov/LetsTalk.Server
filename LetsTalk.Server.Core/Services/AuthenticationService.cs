using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;

namespace LetsTalk.Server.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private const string FACEBOOK = "FACEBOOK";
    private const string VK = "VK";

    private readonly IFacebookService _facebookService;
    private readonly IVkService _vkService;

    public AuthenticationService(
        IFacebookService facebookService,
        IVkService vkService)
    {
        _facebookService = facebookService;
        _vkService = vkService;
    }

    public Task<LoginResponseDto> LoginAsync(LoginServiceInput model, CancellationToken cancellationToken)
    {
        return model.Provider switch
        {
            FACEBOOK => _facebookService.LoginAsync(model, cancellationToken),
            VK => _vkService.LoginAsync(model, cancellationToken),
            _ => throw new BadRequestException("Authorization provider is not suppoted")
        };
    }
}
