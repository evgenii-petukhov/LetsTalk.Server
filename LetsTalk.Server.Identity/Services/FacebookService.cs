using LetsTalk.Server.Abstractions.Authentication;
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

public class FacebookService : IFacebookService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAuthenticationClient _authenticationClient;
    private readonly AuthenticationSettings _authenticationSettings;

    public FacebookService(
        IAccountRepository accountRepository,
        IAuthenticationClient authenticationClient,
        IOptions<AuthenticationSettings> options)
    {
        _accountRepository = accountRepository;
        _authenticationClient = authenticationClient;
        _authenticationSettings = options.Value;
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
                throw new BadRequestException(response.ErrorMessage!);

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
                    AccountTypeId = (int)AccountTypes.Facebook,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Email = data.Email,
                    PhotoUrl = data.Picture!.Data!.Url
                };
                await _accountRepository.CreateAsync(account);
            }

            // generate jwt token to access secure routes on this API
            var token = await _authenticationClient.GenerateJwtToken(_authenticationSettings.Url, account.Id);

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
