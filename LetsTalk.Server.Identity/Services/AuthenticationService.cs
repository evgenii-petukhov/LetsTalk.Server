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
    private readonly IJwtService _jwtService;
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountTypeRepository _accountTypeRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationService(
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings,
        IAccountRepository accountRepository,
        IAccountTypeRepository accountTypeRepository)
    {
        _jwtService = jwtService;
        _accountRepository = accountRepository;
        _accountTypeRepository = accountTypeRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto> Login(LoginServiceInput model)
    {
        // verify access token with facebook API to authenticate
        var client = new RestClient("https://graph.facebook.com/");
        var request = new RestRequest($"{model.Id}?fields=id,email,name,first_name,last_name,picture.type(large)&access_token={model.AuthToken}");
        try
        {
            var response = await client.GetAsync(request);

            if (!response.IsSuccessful)
                throw new AppException(response.ErrorMessage!);

            // get data from response and account from db
            var data = JsonConvert.DeserializeObject<FacebookResponse>(response.Content!)!;
            string externalId = data.Id!;
            var account = await _accountRepository.GetByExternalIdAsync(externalId);

            // create new account if first time logging in
            if (account == null)
            {
                account = new Account
                {
                    ExternalId = externalId,
                    AccountType = await _accountTypeRepository.GetByIdAsync((int)AccountTypes.Facebook),
                    Name = data.Name,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Email = data.Email,
                    PhotoUrl = data.Picture!.Data!.Url
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
        catch
        {
            return new LoginResponseDto
            {
                Success = false
            };
        }
    }
}

