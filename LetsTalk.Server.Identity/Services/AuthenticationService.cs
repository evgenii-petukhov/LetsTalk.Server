using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Abstractions.Repositories;
using LetsTalk.Server.Core.Exceptions;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Identity.Models;
using LetsTalk.Server.Models.Authentication;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace LetsTalk.Server.Identity.Services;

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

    public async Task<LoginResponseDto> Login(LoginServiceInput model)
    {
        if (string.Equals(model.Provider, "FACEBOOK", StringComparison.Ordinal))
        {
            return await _facebookService.Login(model);
        }
        else if (string.Equals(model.Provider, "VK", StringComparison.Ordinal))
        {
            return await _vkService.Login(model);
        }
        else
        {
            throw new BadRequestException("Authorization provider is not suppoted");
        }
    }
}

