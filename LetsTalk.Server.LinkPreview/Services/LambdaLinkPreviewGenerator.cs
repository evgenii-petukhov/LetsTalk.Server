using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Configuration.Models;
using Amazon.Lambda;
using Amazon.Runtime;
using Amazon;
using Amazon.Lambda.Model;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;

namespace LetsTalk.Server.LinkPreview.Services;

public class LambdaLinkPreviewGenerator(
    ILogger<LambdaLinkPreviewGenerator> logger,
    IOptions<AwsSettings> awsOptions) : ILinkPreviewService
{
    private readonly ILogger<LambdaLinkPreviewGenerator> _logger = logger;
    private readonly AwsSettings _awsSettings = awsOptions.Value;

    public async Task<LinkPreviewResponse> GenerateLinkPreviewAsync(string url, CancellationToken cancellationToken)
    {
        using var client = GetLambdaClient();
        var response = await client.InvokeAsync(new InvokeRequest
        {
            FunctionName = "GenerateLinkPreviewAsync",
            Payload = JsonSerializer.Serialize(url)
        }, cancellationToken);

        if (response == null)
        {
            _logger.LogInformation("Title is empty: {url}", url);
            return null!;
        }

        using var sr = new StreamReader(response.Payload);
        return JsonSerializer.Deserialize<LinkPreviewResponse>(await sr.ReadToEndAsync(cancellationToken))!;
    }

    private AmazonLambdaClient GetLambdaClient()
    {
        var awsCredentials = new BasicAWSCredentials(_awsSettings.AccessKey, _awsSettings.SecretKey);
        var lambdaConfig = new AmazonLambdaConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(_awsSettings.Region)
        };
        return new AmazonLambdaClient(awsCredentials, lambdaConfig);
    }
}
