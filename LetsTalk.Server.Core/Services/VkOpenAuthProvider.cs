using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Attributes;
using LetsTalk.Server.Core.Constants;
using LetsTalk.Server.Core.Models.Authentication;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Logging.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Newtonsoft.Json;
using RestSharp;

namespace LetsTalk.Server.Core.Services;

[OpenAuthProviderId(OpenAuthProviderIdentifiers.VK)]
public class VkOpenAuthProvider : IOpenAuthProvider
{
    private const string VK_URL = "https://api.vk.com/";

    private readonly IAppLogger<VkOpenAuthProvider> _appLogger;
    private readonly IAuthenticationClient _authenticationClient;
    private readonly IAccountAgnosticService _accountAgnosticService;

    public VkOpenAuthProvider(
        IAppLogger<VkOpenAuthProvider> appLogger,
        IAuthenticationClient authenticationClient,
        IAccountAgnosticService accountAgnosticService)
    {
        _appLogger = appLogger;
        _authenticationClient = authenticationClient;
        _accountAgnosticService = accountAgnosticService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginServiceInput model, CancellationToken cancellationToken)
    {
        // verify access token with facebook API to authenticate
        using var client = new RestClient(VK_URL);
        var request = new RestRequest($"method/users.get?user_ids={model.Id}&fields=id,first_name,last_name,photo_max&access_token={model.AuthToken}&v=5.131");
        try
        {
            var response = await client.GetAsync(request, cancellationToken);

            if (!response.IsSuccessful)
            {
                throw new BadRequestException(response.ErrorMessage!);
            }

            // get data from response and account from db
            var data = JsonConvert.DeserializeObject<VkResponse>(response.Content!)!;
            _appLogger.LogInformation("VK response: {@response}", response.Content!);
            if (data.Error != null)
            {
                throw new BadRequestException(data.Error.Message!);
            }

            var accountId = await _accountAgnosticService.CreateOrUpdateAsync(
                data.Response![0].Id!,
                AccountTypes.VK,
                data.Response[0].FirstName!,
                data.Response[0].LastName!,
                null!,
                data.Response[0].PictureUrl!,
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
            throw new BadRequestException($"VK response: {e.Message}");
        }
        catch (Exception e)
        {
            throw new BadRequestException(e.Message);
        }
    }
}
