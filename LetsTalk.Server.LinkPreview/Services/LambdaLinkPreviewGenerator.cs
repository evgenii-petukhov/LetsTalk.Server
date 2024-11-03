using LetsTalk.Server.LinkPreview.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using LetsTalk.Server.SignPackage.Abstractions;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Configuration.Models;
using Amazon.Lambda;
using Amazon.Runtime;
using Amazon;
using Amazon.Lambda.Model;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;

namespace LetsTalk.Server.LinkPreview.Services;

public class LambdaLinkPreviewGenerator : LinkPreviewGeneratorBase, ILinkPreviewGenerator
{
    private readonly ILogger<LinkPreviewGenerator> _logger;
    private readonly ISignPackageService _signPackageService;
    private readonly HttpClient _httpClient;
    private readonly ApplicationUrlSettings _applicationUrlSettings;
    private readonly AmazonLambdaClient _lambdaClient;
    private bool _disposedValue;

    public LambdaLinkPreviewGenerator(
        ILogger<LinkPreviewGenerator> logger,
        IHttpClientFactory httpClientFactory,
        ISignPackageService signPackageService,
        IOptions<ApplicationUrlSettings> applicationUrlOptions,
        IOptions<AwsSettings> awsOptions): base(logger, httpClientFactory, signPackageService, applicationUrlOptions)
    {
        _logger = logger;
        _signPackageService = signPackageService;
        _httpClient = httpClientFactory.CreateClient(nameof(LambdaLinkPreviewGenerator));
        _applicationUrlSettings = applicationUrlOptions.Value;
        var awsCredentials = new BasicAWSCredentials(awsOptions.Value.AccessKey, awsOptions.Value.SecretKey);
        var lambdaConfig = new AmazonLambdaConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(awsOptions.Value.Region)
        };
        _lambdaClient = new AmazonLambdaClient(awsCredentials, lambdaConfig);
    }

    public async Task ProcessMessageAsync(string messageId, string chatId, string url)
    {
        var response = await _lambdaClient.InvokeAsync(new InvokeRequest
        {
            FunctionName = "GenerateLinkPreviewAsync",
            Payload = JsonSerializer.Serialize(url)
        });

        if (response == null)
        {
            _logger.LogInformation("Title is empty: {url}", url);
            return;
        }

        using var sr = new StreamReader(response.Payload);
        var model = JsonSerializer.Deserialize<LinkPreviewResponse>(await sr.ReadToEndAsync());

        await HandleResponseAsync(model, messageId, chatId, url);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _lambdaClient.Dispose();
            }

            _disposedValue = true;
        }
        base.Dispose(disposing);
    }
}
