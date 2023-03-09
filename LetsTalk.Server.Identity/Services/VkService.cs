using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Abstractions.Logging;
using LetsTalk.Server.Abstractions.Repositories;
using LetsTalk.Server.Core.Exceptions;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Identity.Models;
using LetsTalk.Server.Models.Account.Enums;
using LetsTalk.Server.Models.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace LetsTalk.Server.Identity.Services;

public class VkService : IVkService
{
    private readonly IJwtService _jwtService;
    private readonly IAccountRepository _accountRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly IAppLogger<VkService> _appLogger;

    public VkService(
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings,
        IAccountRepository accountRepository,
        IAppLogger<VkService> appLogger)
    {
        _jwtService = jwtService;
        _accountRepository = accountRepository;
        _jwtSettings = jwtSettings.Value;
        _appLogger = appLogger;
    }

    public async Task<LoginResponseDto> Login(LoginServiceInput model)
    {
        // verify access token with facebook API to authenticate
        var client = new RestClient("https://api.vk.com/");
        var request = new RestRequest($"method/users.get?user_ids={model.Id}&fields=id,first_name,last_name,photo_max&access_token={model.AuthToken}&v=5.131");
        try
        {
            var response = await client.GetAsync(request);

            if (!response.IsSuccessful)
                throw new BadRequestException(response.ErrorMessage!);

            // get data from response and account from db
            var data = JsonConvert.DeserializeObject<VkResponse>(response.Content!)!;
            _appLogger.LogInformation("VK response: {@response}", response.Content!);
            if (data.Error != null)
            {
                throw new BadRequestException(data.Error.Message!);
            }

            string externalId = data.Response![0].Id!;
            var account = await _accountRepository.GetByExternalIdAsync(externalId);

            // create new account if first time logging in
            if (account == null)
            {
                account = new Account
                {
                    ExternalId = externalId,
                    AccountTypeId = (int)AccountTypes.VK,
                    FirstName = data.Response[0].FirstName,
                    LastName = data.Response[0].LastName,
                    PhotoUrl = data.Response[0].PictureUrl
                };
                await _accountRepository.CreateAsync(account);
            }

            // generate jwt token to access secure routes on this API
            var token = _jwtService.GenerateJwtToken(account.Id);

            return new LoginResponseDto
            {
                Success = true,
                Token = token
            };
        }
        catch (HttpRequestException e)
        {
            throw new BadRequestException($"Facebook response: {e.Message}");
        }
        catch (Exception e)
        {
            throw new BadRequestException(e.Message);
        }
    }
}
