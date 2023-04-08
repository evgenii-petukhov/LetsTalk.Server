using LetsTalk.Server.API.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;

namespace LetsTalk.Server.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IFacebookService _facebookService;
    private readonly IVkService _vkService;

    public AuthenticationService(
        IFacebookService facebookService,
        IVkService vkService)
    {
        _facebookService = facebookService;
        _vkService = vkService;
    }

    public Task<LoginResponseDto> Login(LoginServiceInput model)
    {
        if (string.Equals(model.Provider, "FACEBOOK", StringComparison.Ordinal))
        {
            return _facebookService.Login(model);
        }
        else if (string.Equals(model.Provider, "VK", StringComparison.Ordinal))
        {
            return _vkService.Login(model);
        }
        else
        {
            throw new BadRequestException("Authorization provider is not suppoted");
        }
    }
}

