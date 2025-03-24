using System.Text.Json;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Configuration.Models;
using Amazon.Lambda;
using Amazon.Runtime;
using Amazon;
using Amazon.Lambda.Model;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;
using LetsTalk.Server.ImageProcessing.Utility.Models;

namespace LetsTalk.Server.ImageProcessing.Service.Services;

public class LambdaImageProcessingService(
    IOptions<AwsSettings> awsOptions) : IImageProcessingService
{
    private readonly AwsSettings _awsSettings = awsOptions.Value;

    public async Task<ProcessImageResponse> ProcessImageAsync(string imageId, int maxWidth, int maxHeight, CancellationToken cancellationToken)
    {
        using var client = GetLambdaClient();
        var response = await client.InvokeAsync(new InvokeRequest
        {
            FunctionName = "ImageProcessingLambda_ProcessImageAsync",
            Payload = JsonSerializer.Serialize(new ProcessImageRequest
            {
                FileName = imageId,
                BucketName = _awsSettings.BucketName,
                MaxWidth = maxWidth,
                MaxHeight = maxHeight
            })
        }, cancellationToken);

        if (response == null)
        {
            return null!;
        }

        using var sr = new StreamReader(response.Payload);
        return JsonSerializer.Deserialize<ProcessImageResponse>(await sr.ReadToEndAsync(cancellationToken))!;
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
