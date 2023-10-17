using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Attributes;
using LetsTalk.Server.Core.Constants;
using LetsTalk.Server.Core.Models.Authentication;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Enums;
using Newtonsoft.Json;
using RestSharp;

namespace LetsTalk.Server.Core.Services;

[OpenAuthProviderId(OpenAuthProviderIdentifiers.FACEBOOK)]
public class FacebookOpenAuthProvider : IOpenAuthProvider
{
    private const string FACEBOOK_URL = "https://graph.facebook.com/";

    private readonly IAuthenticationClient _authenticationClient;
    private readonly IAccountDataLayerService _accountDataLayerService;

    public FacebookOpenAuthProvider(
        IAuthenticationClient authenticationClient,
        IAccountDataLayerService accountDataLayerService)
    {
        _authenticationClient = authenticationClient;
        _accountDataLayerService = accountDataLayerService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginServiceInput model, CancellationToken cancellationToken)
    {
        // verify access token with facebook API to authenticate
        using var client = new RestClient(FACEBOOK_URL);
        var request = new RestRequest($"{model.Id}?fields=id,email,name,first_name,last_name,picture.type(large)&access_token={model.AuthToken}");
        try
        {
            var response = await client.GetAsync(request, cancellationToken: cancellationToken);

            if (!response.IsSuccessful)
            {
                throw new BadRequestException(response.ErrorMessage!);
            }

            // get data from response and account from db
            var data = JsonConvert.DeserializeObject<FacebookResponse>(response.Content!)!;

            var accountId = await _accountDataLayerService.CreateOrUpdateAsync(
                data.Id!,
                AccountTypes.Facebook,
                data.FirstName!,
                data.LastName!,
                data.Email!,
                data.Picture!.Data!.Url!,
                cancellationToken);

            // generate jwt token to access secure routes on this API
            var token = await _authenticationClient.GenerateJwtTokenAsync(accountId);

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
