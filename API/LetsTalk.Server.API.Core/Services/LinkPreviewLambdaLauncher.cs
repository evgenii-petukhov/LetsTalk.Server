using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LetsTalk.Server.API.Core.Services;

public class LinkPreviewLambdaLauncher(
    IOptions<AwsSettings> awsOptions,
    IOptions<ApplicationUrlSettings> applicationSettingsOptions,
    IOptions<LinkPreviewSettings> linkPreviewSettingsOptions) : ILinkPreviewLauncher
{
    private readonly AwsSettings _awsSettings = awsOptions.Value;
    private readonly ApplicationUrlSettings _applicationUrlSettings = applicationSettingsOptions.Value;
    private readonly LinkPreviewSettings _linkPreviewSettings = linkPreviewSettingsOptions.Value;

    public async Task LaunchAsync(
        string messageId,
        string url,
        string chatId,
        string token,
        CancellationToken cancellationToken)
    {
        using var client = GetLambdaClient();
        var response = await client.InvokeAsync(new InvokeRequest
        {
            FunctionName = "LinkPreviewLambda_GenerateAsync",
            Payload = JsonSerializer.Serialize(new LinkPreviewRequest
            {
                MessageId = messageId,
                Url = url,
                ChatId = chatId,
                Token = token,
                SecretKey = _linkPreviewSettings.SecretKey,
                ApiUrl = _applicationUrlSettings.Api,
            }),
            InvocationType = InvocationType.Event
        }, cancellationToken);
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
