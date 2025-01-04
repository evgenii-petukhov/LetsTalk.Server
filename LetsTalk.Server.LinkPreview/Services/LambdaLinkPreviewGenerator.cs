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
    private static readonly Action<ILogger, string, Exception?> _logTitleEmpty =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, nameof(LambdaLinkPreviewGenerator)), "Title is empty: {Url}");

    private readonly ILogger<LambdaLinkPreviewGenerator> _logger = logger;
    private readonly AwsSettings _awsSettings = awsOptions.Value;

    public async Task<LinkPreviewResponse> GenerateLinkPreviewAsync(string url, CancellationToken cancellationToken)
    {
        using var client = GetLambdaClient();
        var response = await client.InvokeAsync(new InvokeRequest
        {
            FunctionName = "LinkPreviewLambda_GenerateAsync",
            Payload = JsonSerializer.Serialize(url)
        }, cancellationToken);

        if (response == null)
        {
            _logTitleEmpty(_logger, url, null);
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
