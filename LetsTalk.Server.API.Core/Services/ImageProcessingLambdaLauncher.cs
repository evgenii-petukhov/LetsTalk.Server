using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.ImageProcessing.Utility.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LetsTalk.Server.API.Core.Services;

public class ImageProcessingLambdaLauncher(
    IOptions<AwsSettings> awsOptions,
    IOptions<ApplicationUrlSettings> applicationSettingsOptions,
    IOptions<ImageConstraints> imageConstraintsOptions
) : IImageProcessingLauncher
{
    private readonly AwsSettings _awsSettings = awsOptions.Value;
    private readonly ApplicationUrlSettings _applicationUrlSettings = applicationSettingsOptions.Value;
    private readonly ImageConstraints _imageConstraints = imageConstraintsOptions.Value;

    public async Task LaunchAsync(
        string messageId,
        string imageId,
        string chatId,
        int fileStorageTypeId,
        string token,
        CancellationToken cancellationToken = default)
    {
        using var client = GetLambdaClient();
        var response = await client.InvokeAsync(new InvokeRequest
        {
            FunctionName = "ImageProcessingLambda_ProcessImageAsync",
            Payload = JsonSerializer.Serialize(new ProcessImageRequest
            {
                FileName = imageId,
                BucketName = _awsSettings.BucketName,
                ApiUrl = _applicationUrlSettings.Api,
                MessageId = messageId,
                ChatId = chatId,
                FileStorageTypeId = fileStorageTypeId,
                Token = token,
                MaxWidth = _imageConstraints.ImagePreviewMaxWidth,
                MaxHeight = _imageConstraints.ImagePreviewMaxHeight,
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
