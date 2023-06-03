using LetsTalk.Server.Exceptions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Logging.Abstractions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Persistence.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Models.Login;

namespace LetsTalk.Server.Core.Services;

public class VkService : IVkService
{
    private const string VK_URL = "https://api.vk.com/";

    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IAppLogger<VkService> _appLogger;
    private readonly IAuthenticationClient _authenticationClient;
    private readonly AuthenticationSettings _authenticationSettings;

    public VkService(
        IAccountDataLayerService accountDataLayerService,
        IAppLogger<VkService> appLogger,
        IAuthenticationClient authenticationClient,
        IOptions<AuthenticationSettings> options)
    {
        _accountDataLayerService = accountDataLayerService;
        _appLogger = appLogger;
        _authenticationClient = authenticationClient;
        _authenticationSettings = options.Value;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginServiceInput model, CancellationToken cancellationToken)
    {
        // verify access token with facebook API to authenticate
        var client = new RestClient(VK_URL);
        var request = new RestRequest($"method/users.get?user_ids={model.Id}&fields=id,first_name,last_name,photo_max&access_token={model.AuthToken}&v=5.131");
        try
        {
            var response = await client.GetAsync(request, cancellationToken: cancellationToken);

            if (!response.IsSuccessful)
                throw new BadRequestException(response.ErrorMessage!);

            // get data from response and account from db
            var data = JsonConvert.DeserializeObject<VkResponse>(response.Content!)!;
            _appLogger.LogInformation("VK response: {@response}", response.Content!);
            if (data.Error != null)
            {
                throw new BadRequestException(data.Error.Message!);
            }

            var accountId = await _accountDataLayerService.CreateOrUpdateAsync(
                data.Response![0].Id!,
                AccountTypes.VK,
                data.Response[0].FirstName,
                data.Response[0].LastName,
                null,
                data.Response[0].PictureUrl,
                cancellationToken);

            // generate jwt token to access secure routes on this API
            var token = await _authenticationClient.GenerateJwtTokenAsync(_authenticationSettings.Url!, accountId);

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
